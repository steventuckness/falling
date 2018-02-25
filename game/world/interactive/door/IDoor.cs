using Godot;
using System;

public interface IDoor {
    void Open();
    void Close();
    bool IsOpen();
    bool CanOpen();
}
