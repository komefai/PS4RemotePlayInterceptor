# PS4 Remote Play Interceptor

A small .NET library to intercept controls on PS4 Remote Play for Windows, powered by [EasyHook](https://easyhook.github.io/).

## Install

#### Using NuGet
```
Install-Package PS4RemotePlayInterceptor
```

#### From Source
Add reference PS4RemotePlayInterceptor.dll to your project.

## Example Usage

This console application will hold the X button while moving the left analog stick upwards until interrupted by a keypress.

```csharp
using PS4RemotePlayInterceptor;

class Program
{
    static void Main(string[] args)
    {
        // Inject into PS4 Remote Play
        Interceptor.Callback = new InterceptionDelegate(OnReceiveData);
        Interceptor.Inject();

        Console.ReadKey();
    }

    private static void OnReceiveData(ref DualshockState state)
    {
        /* -- Modify the controller state here -- */

        // Force press X
        state.Cross = true;

        // Force left analog upwards
        state.LY = 0;

        // Force left analog downwards
        // state.LY = 255;

        // Force left analog to center
        // state.LX = 128;
        // state.LY = 128;
    }
}
```

See [prototype demo](https://youtu.be/QjTZsPR-BcI) made using this library.

## Troubleshoot

> {"STATUS_INTERNAL_ERROR: Unknown error in injected C++ completion routine. (Code: 15)"}

SOLUTION: Restart PS4 Remote Play.

## Credits

- https://easyhook.github.io/
- https://github.com/Jays2Kings/DS4Windows
- http://www.psdevwiki.com/ps4/DS4-USB