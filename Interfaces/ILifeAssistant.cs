using System.Collections.Generic;
public interface ILifeCyclable {
    void Initialize();
    void Load();
    void Start();
    void Pause();
    void Resume();
    void Stop();
    void Unload();
    void Quit();
    void Kill();
}