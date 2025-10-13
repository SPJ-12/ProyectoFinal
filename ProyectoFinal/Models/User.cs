using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class User : INotifyPropertyChanged
{
    private string _id;
    private string _username;
    private string _email;
    private DateTime _registrationDate;
    private bool _isActive;

    public User()
    {
        _id = Guid.NewGuid().ToString();
        _registrationDate = DateTime.Now;
        _isActive = true;
    }

    public User(string username, string email) : this()
    {
        _username = username;
        _email = email;
    }

    public string Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged();
            }
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime RegistrationDate
    {
        get => _registrationDate;
        set
        {
            if (_registrationDate != value)
            {
                _registrationDate = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

