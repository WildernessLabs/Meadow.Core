// See https://aka.ms/new-console-template for more information
public interface IUpdateHandler
{
    void OnUpdateAvailable(UpdateInfo info);
    void RetrieveUpdate();
    void ApplyUpdate();
    void OnUpdateSuccess();
    void OnUpdateFailure();
}