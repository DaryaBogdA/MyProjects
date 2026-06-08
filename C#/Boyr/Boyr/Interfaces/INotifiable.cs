using System;

namespace Boyr.Interfaces
{
    public interface INotifiable
    {
        void ShowSuccess(string message);
        void ShowError(string message);
        void ShowWarning(string message);
        void ShowInfo(string message);
    }
}