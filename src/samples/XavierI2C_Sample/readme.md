# Example for Containerizing a Meadow.Linux Application

This example assumes:
- You are building the container on the target device (an AGX Xavier)
- You have Docker already installed
- You have docker-compose already installed
- You're building Meadow.Linux and your app from source

## Ensure the hardware is working

```
$ i2cdetect -y -r 8
     0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f
00:          -- -- -- -- -- -- -- -- -- -- -- -- --
10: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
20: -- -- -- -- -- -- -- -- 28 -- -- -- -- -- -- --
30: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
40: 40 -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
50: -- -- -- 53 -- -- -- -- -- -- 5a -- -- -- -- --
60: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
70: -- -- -- -- -- -- -- --
```

## Copy the source to the target

Since the Dockerfile has path dependencies in it, it is important that everything is in the proper place.

- Create a remote directory on the AGX at `~/m4l`
- Copy the `Meadow.Linux` source folder (named `lib`) over to `~\m4l\lib`
- Copy the `XavierI2C_Sample` source to `~/m4l/samples/XavierI2C_Sample`

On the target you will effectively have mirrored the repo source into the `~/m4l` folder.

> TODO: git clone into the target

## Build the container image

Building the image is a straightforward `docker` command.  Since we need to build multiple projects, we need to build from the source tree root and use the `-f` flag.  You *cannot* build this from the child folder where the Dockerfile resides

```
$ cd ~/m4l
$ sudo docker build -t sensors -f samples/XavierI2C_Sample/Dockerfile .
```


## Running the container

Because the container needs access to hardware resources, it's not just a simple `docker run` command.  You need to give the container access to the device driver with the `--device /dev/i2c-8` parameter, and you must have the docker container running in a group that has access to the bus.  The group is `i2c` and on my system the GID for that group is 108.

```
$ sudo docker run -it --device /dev/i2c-8 --group-add 108 sensors
```

If you run into "access denied" issues, check that your `i2c` group has the same GID:

```
$ getent group i2c
i2c:x:108:my_user_name
      ^^^ [this is the GID]
```

A simpler way to run is to use `docker-compose`

```
$ sudo docker-compose -f samples/XavierI2C_Sample/docker-compose.yml up
```