# Meadow.Core → net10.0 Modernization Plan

**Branch:** `meadow-3.0`
**Constraint:** Existing public API must not be changed or removed. New overloads and types may be added.

## Context

Meadow.Core currently dual-targets `netstandard2.1` + `net9.0`. The plan is to **drop netstandard2.1 entirely** and retarget all projects to `net10.0`. This is a clean break — no `#if` guards, no backward compatibility shims. The STM32 F7 MCU is getting a real modern .NET runtime, and the codebase should fully embrace it.

This unlocks runtime features that netstandard2.1 could never provide (source-generated interop, JIT optimizations, stack allocation, modern GC), plus API surface that was awkward to polyfill.

---

## API Stability Rule

**Do not change or remove any existing public API.** All improvements are additive:
- Add new overloads (e.g. `ReadOnlySpan<byte>` variants alongside existing `Span<byte>` methods)
- Add new types (inline array structs, Lock fields)
- Internal/private code can be freely refactored
- Use `[OverloadResolutionPriority]` to steer callers toward new overloads without breaking old ones
- Interface additions require default interface methods to avoid breaking implementors

---

## 0. TFM Migration — The Prerequisite

**Retarget all projects to `net10.0`** and clean up the build:

### Meadow.Core.csproj
- `<TargetFrameworks>netstandard2.1;net9.0</TargetFrameworks>` → `<TargetFramework>net10.0</TargetFramework>`
- `<LangVersion>12.0</LangVersion>` → `<LangVersion>preview</LangVersion>` (C# 14)
- Remove `Condition="'$(TargetFramework)' == 'net9.0'"` compile exclusions for cloud files — decide: bring cloud code forward or factor it out properly
- Remove conditional package references: `System.IO.Hashing`, `System.IO.Ports`, `sqlite-net-static` (all were netstandard2.1-only)
- Remove `MQTTnet` conditional — either include or exclude unconditionally
- **Fix nullable warnings properly** instead of suppressing them (`NoWarn CS8597;CS8600-8604...` can be removed once nullability is cleaned up)

### Meadow.F7.csproj
- Same TFM change, remove net9.0 conditional cloud exclusions
- Remove CA2200/CA2265 suppressions — fix the actual warnings

### Other projects
- `Meadow.Linux`, `Meadow.Windows`, `Meadow.Mac`: `net8.0` → `net10.0`
- `Meadow.Simulation`: `netstandard2.1` → `net10.0`
- `Meadow.Desktop`, UI projects, test projects: all → `net10.0`
- `SimulatorHost` (`net6.0`) → `net10.0`
- Test projects (`netcoreapp3.1`, `net5.0`, `net8.0`) → `net10.0`

---

## What .NET 10 Adds Over .NET 9

Most features in this plan are available from .NET 8/9. But targeting 10 specifically gives:

| Feature | Why it matters for Meadow |
|---------|--------------------------|
| **C# 14 `extension` types** | Add methods/properties to hardware interfaces (`IPin`, `II2cBus`) without wrapper classes or allocating adapter objects — cleaner driver APIs |
| **Improved JIT escape analysis** | More short-lived objects automatically stack-allocated by the runtime — free perf for code you don't even change |
| **Better ARM codegen** | Direct throughput improvement on the Cortex-M7; every .NET release improves ARM JIT |
| **`field` keyword (stable)** | Cleaner auto-properties with validation — reduces boilerplate in port/pin classes |
| **`OverloadResolutionPriority`** | Add new `Span<byte>` overloads that the compiler prefers over old `byte[]` overloads — smooth migration for Meadow.Foundation drivers without breaking existing API |
| **GC region improvements** | Better memory management for small heaps — directly relevant to MCU RAM constraints |
| **NativeAOT maturity** | Future option: AOT compile the whole app → zero JIT warmup, smaller working set |
| **`params ReadOnlySpan<T>` broadly usable** | Variadic methods without array allocation — useful for multi-register I2C writes |

---

## 1. Source-Generated P/Invoke (`LibraryImport` replacing `DllImport`)

**Impact: High — eliminates runtime marshaling stubs, reduces startup cost, enables Span-native signatures**

~70+ `[DllImport]` declarations across the F7 interop layer (all internal, so no API concern):
- `Interop.ioctl.cs` (~30 overloads), `Interop.read.cs`, `Interop.write.cs`, `Interop.open.cs`, `Interop.close.cs`
- `Interop.queue.cs` (~12), `Interop.signal.cs` (~7)
- `Interop.mount.cs`, `Interop.mallinfo.cs`, `Interop.cell.cs`, `Interop.meadow_adc.cs`, `Interop.freq.cs`, etc.

**What changes:**
- `[DllImport(...)]` + `static extern` → `[LibraryImport(...)]` + `static partial`
- `byte[]` parameters → `Span<byte>` / `ReadOnlySpan<byte>` — callers pass stack-allocated or pooled buffers without pinning
- Critical signatures:
  - `Interop.Nuttx.read(IntPtr handle, byte[] buf, int n)` → accepts `Span<byte>`
  - `Interop.Nuttx.write(IntPtr handle, byte[] buf, int count)` → accepts `ReadOnlySpan<byte>`
- String params (`CharSet.Ansi`) → `StringMarshalling = StringMarshalling.Utf8`

**Files:** `source/implementations/f7/Meadow.F7/Interop/Interop.*.cs`, `source/implementations/linux/Meadow.Linux/Interop/Interop.cs`

---

## 2. Eliminate Hot-Path Heap Allocations (`Span<T>`, `stackalloc`, `ArrayPool`)

**Impact: Very High — directly reduces GC pressure on a memory-constrained MCU**

### 2a. SerialPortBase — `.ToArray()` defeating Span (CRITICAL)

`SerialPortBase.cs:393-394`:
```csharp
Span<byte> data = buffer.AsSpan().Slice(currentIndex, bytesToWriteThisLoop);
result = WriteHardwarePort(_driverHandle, data.ToArray(), count);  // allocates every write!
```
Once internal `WriteHardwarePort` accepts `ReadOnlySpan<byte>` via LibraryImport:
```csharp
result = WriteHardwarePort(_driverHandle, buffer.AsSpan(currentIndex, bytesToWriteThisLoop), count);
```

### 2b. SerialPortBase — read buffer pooling

`SerialPortBase.cs:425`: `var readBuffer = new byte[4096]` — Convert to `ArrayPool<byte>.Shared.Rent(4096)` with return in finally/close.

### 2c. Esp32Coprocessor Encoders — 80+ allocations (all internal)

`Encoders.cs` creates `new byte[N]` for every encode/decode. Change to:
- Small buffers (< 256 bytes): `stackalloc byte[N]`
- Larger buffers: `ArrayPool<byte>.Shared.Rent(N)` with try/finally return
- Refactor internal encode methods to accept `Span<byte>` destination instead of returning `byte[]`

### 2d. Esp32Coprocessor SPI buffers (internal)

- `Esp32Coprocessor.cs:265`: `new byte[22]` → `stackalloc byte[22]`
- `Esp32Coprocessor.cs:385`: `new byte[MAXIMUM_SPI_BUFFER_LENGTH]` → `ArrayPool` rental

### 2e. Cloud update buffers (internal)

`MeadowCloudUpdateService.cs:42-43`: Two permanently-allocated 384KB buffers → `ArrayPool<byte>.Shared.Rent()` on download start, return on completion. Saves 768KB when idle.

### 2f. Configuration buffers (internal)

`F7PlatformOS.Configuration.cs:216,249,268`: Small config buffers → `stackalloc`

**Key files:**
- `source/Meadow.Core/Hardware/Communications/SerialPortBase.cs`
- `source/implementations/f7/Meadow.F7/Devices/Esp32Coprocessor/Encoders.cs`
- `source/implementations/f7/Meadow.F7/Devices/Esp32Coprocessor/Esp32Coprocessor.cs`
- `source/implementations/f7/Meadow.F7/Devices/Esp32Coprocessor/Esp32WiFiAdapter.cs`
- `source/implementations/f7/Meadow.F7/F7PlatformOS.Configuration.cs`

---

## 3. `System.Threading.Lock` (internal fields only — no API change)

**Impact: Medium — lighter than `lock(object)`, avoids Monitor overhead**

```csharp
// Before (private/internal field):
private object _accessLock = new();
// After:
private readonly Lock _accessLock = new();
```

**High-frequency lock sites:**
- `SerialPortBase.cs:39` — `_accessLock`
- `F7GPIOManager.cs` — `_portPinCache`, `_currentConfigs`
- `F7GPIOManager_interrupts.cs` — `_configuredInterrupts`, `_interruptGroupsInUse`
- `DeviceChannelManager.cs` — `_channelLock`
- `Esp32Coprocessor.cs` — `_queuedEthernetEvents`
- `Esp32WiFiAdapter.cs` — `_lock`
- `F7PlatformOS.PowerController.cs` — `_sleepAwarePeripherals`

---

## 4. `FrozenDictionary` / `FrozenSet` for Read-Heavy Lookup Tables (internal)

**Impact: Medium — faster lookups via perfect hashing, inherently thread-safe**

**Candidates:**
- `F7GPIOManager.cs:153-158` — `_portPinCache`. Build mutable during init, then `.ToFrozenDictionary()`. Eliminates the lock.
- `DeviceChannelManager.cs:27` — `_channelStates`
- Pin channel lists in `F7FeatherV2.Pinout.cs` — 30+ `new List<IChannelInfo>` → frozen collections
- `F7GPIOManager_interrupts.cs` — `_configuredInterrupts` (if it stabilizes post-init)

---

## 5. `ReadOnlySpan<byte>` Overloads for Write-Direction Bus APIs

**Impact: Medium — enables `stackalloc`, string literals, and const data in write calls**

**API approach: ADD new overloads, keep existing ones:**
```csharp
// Existing (keep):
void Write(byte peripheralAddress, Span<byte> writeBuffer);
// New overload (add):
[OverloadResolutionPriority(1)]
void Write(byte peripheralAddress, ReadOnlySpan<byte> writeBuffer);
```

Enables callers to do:
```csharp
bus.Write(addr, stackalloc byte[] { 0x01, 0x02 });
bus.Write(addr, "HELLO"u8);
```

Existing code calling with `Span<byte>` continues to work (implicit conversion to `ReadOnlySpan<byte>`). `OverloadResolutionPriority` steers new code toward the better overload.

**Files:** Interfaces in Meadow.Contracts (add default interface methods) + implementations in F7 and Linux.

---

## 6. `Inline Arrays` for Fixed-Size Protocol Buffers

**Impact: Medium — typed, stack-allocated fixed-size buffers**

```csharp
[InlineArray(6)]
public struct MacAddress { private byte _element; }

[InlineArray(22)]
internal struct Esp32RxHeader { private byte _element; }
```

New public types are additive. Internal types are free to add.

**Candidates:** MAC addresses (`new byte[6]` x5+), SPI headers (22 bytes), I2C command buffers, CRC scratch buffers.

---

## 7. `ValueTask` / `ValueTask<T>` — New Overloads Only

**Impact: Medium — avoids Task allocation when result is synchronously available**

**API approach: ADD new methods returning ValueTask, keep existing Task-returning methods:**
```csharp
// Existing (keep):
Task<Voltage> Read();
// New (add):
ValueTask<Voltage> ReadAsync();
```

Or add default interface methods in Meadow.Contracts that delegate to the existing Task methods:
```csharp
ValueTask<Voltage> ReadValueAsync() => new(Read());
```

Implementations can then override with optimized versions.

---

## 8. Fix Async Anti-Patterns (internal — no API change)

**Impact: Medium — prevents threadpool starvation and deadlocks on MCU**

- `F7 I2cBus.cs:87,127,166` — `_busSemaphore.Wait()` → `WaitAsync()` or replace with `Lock`
- `F7FeatherV2.cs:64` — `.Read().Result` blocks on async (deadlock risk)
- `F7GPIOManager_adc.cs:232` — `Task.Delay(100).Wait()` blocks thread
- `F7PlatformOS.PowerController.cs:80` — `.BeforeSleep().Wait()`
- `SoftPwmPort.cs:131` — `async` lambda in `new Thread()` (exceptions lost)

---

## 9. `CollectionsMarshal` for Hot Dictionary Paths (internal)

**Impact: Low-Medium — eliminates double-lookup**

`F7GPIOManager.cs:147-158` — called on every GPIO set/get:
```csharp
if (_portPinCache.ContainsKey(key))
    return (_portPinCache[key].Item1, ...);  // 4 lookups for 1 get!
```
→ `TryGetValue` at minimum, `CollectionsMarshal.GetValueRefOrAddDefault()` ideally.

---

## 10. `GC.AllocateUninitializedArray<T>()` for Native Read Buffers (internal)

**Impact: Low-Medium — skips zero-fill for buffers immediately overwritten by native reads**

---

## 11. Zero-Alloc Logging (internal + additive API in Meadow.Logging)

**Impact: Low-Medium**

- Add `InterpolatedStringHandler`-accepting overloads to `Resolver.Log` (new overloads, keeps existing)
- Guard expensive `.ToString()` in trace logging
- Cache `pin.Key.ToString()` in `F7GPIOManager.cs:149`

---

## 12. `NativeMemory` for Long-Lived Interop Buffers (internal)

**Impact: Low**

- `F7AnalogInputArray.cs` — `GCHandle.Alloc` → `NativeMemory.AlignedAlloc`
- ESP32 SPI exchange buffers

---

## 13. `SearchValues<byte>` + UTF-8 Literals (internal)

**Impact: Low**

- Serial message delimiter detection
- `"CMD"u8` replacing `Encoding.UTF8.GetBytes()`

---

## 14. `params ReadOnlySpan<T>` — New Overloads

**Impact: Low — additive API for variadic hardware writes**

```csharp
// New overload (add):
void WriteRegisters(byte addr, params ReadOnlySpan<byte> values);
// Existing byte[] methods unchanged
```

---

## Implementation Phases

### Phase A — TFM Migration + Foundation
1. Retarget all projects to `net10.0`, remove all conditional compilation
2. `LangVersion` → `preview` (C# 14)
3. Clean up nullable warnings properly (remove `NoWarn` suppressions)
4. `LibraryImport` conversion for F7 + Linux interop layers
5. Add `ReadOnlySpan<byte>` overloads on bus APIs (keep existing)

### Phase B — Allocation Reduction (biggest perf wins)
6. `stackalloc` + `ArrayPool` in Encoders, ESP32 SPI, Serial paths
7. Remove `.ToArray()` in SerialPortBase write path (depends on #4)
8. `Inline arrays` for MAC addresses, protocol headers
9. `GC.AllocateUninitializedArray` for native read buffers

### Phase C — Concurrency & Lookup Optimization
10. `System.Threading.Lock` across all lock sites
11. `FrozenDictionary` for pin caches and config tables
12. `CollectionsMarshal` / `TryGetValue` for GPIO port/pin lookup
13. Fix async anti-patterns (`.Result`, `.Wait()`, async-in-Thread)

### Phase D — API Additions & Smaller Wins
14. `ValueTask` overloads (additive, coordinate with Meadow.Contracts)
15. Zero-alloc logging overloads (coordinate with Meadow.Logging)
16. `UTF-8 literals`, `SearchValues`, `NativeMemory`, `params ReadOnlySpan`
17. Explore `extension` types (C# 14) for hardware interface ergonomics

---

## Verification

- **Build:** `dotnet build` for `net10.0` — clean build, zero warnings
- **Unit tests:** `source/Tests/Core.Unit.Tests/` — full pass
- **API compat:** Verify no existing public API signatures changed or removed
- **Integration:** Deploy to F7 Feather, run sensor + serial + WiFi + cloud scenarios
- **Memory:** Compare GC collection counts and heap size before/after on device
- **Regression:** Ensure Meadow.Foundation drivers still compile without changes

---
---

## Worklog

### 2026-03-30 — Initial planning

- Created branch `meadow-3.0` from `develop`
- Completed full codebase exploration:
  - Cataloged all .csproj files and their TFMs
  - Found ~70+ DllImport declarations in F7 interop layer
  - Found 80+ byte[] allocations in Encoders.cs
  - Found `.ToArray()` in SerialPortBase defeating Span usage on every serial write
  - Found 768KB permanently allocated cloud update buffers
  - Found ~15 lock sites using `lock(object)` pattern
  - Found double-dictionary-lookup in `GetPortAndPin` (called on every GPIO set/get)
  - Found async anti-patterns: `.Result`, `.Wait()`, async-in-Thread
  - Confirmed Span<byte> already used in I2C/SPI bus APIs (good foundation)
  - Confirmed no ArrayPool/MemoryPool usage anywhere in codebase
- Wrote this plan with API stability constraint: add only, never change/remove

### 2026-03-30 — Phase A + B + C implementation (57 files, +513 -455 lines)

**TFM Migration (16 .csproj files):**
- All projects retargeted from netstandard2.1/net5.0/net6.0/net8.0/net9.0 → `net10.0`
- LangVersion → `preview` (C# 14) across all projects
- Removed all TFM-conditional compilation (cloud file exclusions, conditional packages)
- Removed conditional package refs: System.IO.Hashing, System.IO.Ports, sqlite-net-static, MQTTnet
- Kept NoWarn suppressions temporarily (nullable cleanup is separate work)

**LibraryImport conversion (75 declarations across 25 files):**
- F7 interop: 20 files, 75 `[DllImport]` → `[LibraryImport]` with `static partial`
- Linux interop: 5 files (Interop.cs, Interop.Time.cs, Interop.ioctl.cs, Gpiod2.Interop.cs, Gpiod3.Interop.cs)
- String params converted from `CharSet.Ansi` → `StringMarshalling.Utf8`
- `[MarshalAs(UnmanagedType.LPStr)]` removed where redundant
- byte[] params left as-is (Span conversion is Phase B follow-up)

**System.Threading.Lock (15 lock sites across 13 files):**
- Core: SerialPortBase, AnalogInputPort, SqliteTelemetryStore, InMemoryTelemetryStore
- F7: Esp32WiFiAdapter, DeviceChannelManager, F7GPIOManager (+_cacheLock), F7MicroBase
- New Lock fields added for collection-locked patterns: _currentConfigsLock, _configuredInterruptsLock, _interruptGroupsInUseLock, _queuedEthernetEventsLock, _sleepAwarePeripheralsLock, _spiBusCacheLock, _i2cBusCacheLock
- SqliteTelemetryStore: Monitor.TryEnter/Exit → Lock.TryEnter/Exit

**Allocation reduction:**
- SerialPortBase: Added `WriteHardwarePort(handle, buffer, offset, count)` overload using ArrayPool (eliminates .ToArray() on every offset write)
- SerialPortBase: Read buffer changed from `new byte[4096]` → `ArrayPool.Rent` with Return in finally
- Esp32Coprocessor: 3 hot-path allocations → ArrayPool (rxBuffer, encodedResult, battery level buffer)
- Esp32WiFiAdapter: 3 large buffer allocations → ArrayPool (scan, connect, antenna)
- F7GPIOManager.GetPortAndPin: Fixed triple-lookup (ContainsKey + 3x indexer) → TryGetValue, Dictionary<string, Tuple<>> → Dictionary<string, ValueTuple>, AND fixed bug where cache was never populated (always empty!)

**Async anti-patterns fixed:**
- F7GPIOManager_adc: `Task.Delay(100).Wait()` → `Thread.Sleep(100)`
- SoftPwmPort: `async` lambda in `new Thread()` → synchronous lambda with `Thread.Sleep`

**FrozenDictionary evaluation:**
- All internal dictionaries are lazily populated — not suitable for FrozenDictionary
- Pin channel lists need Meadow.Contracts changes — deferred to Phase D
