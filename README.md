# 🖥 Teams Activity Simulator

A lightweight Windows Forms tray application that simulates minimal mouse and keyboard activity.

> Built with .NET (WinForms) using `SendInput` and `keybd_event` from `user32.dll`.

---

## ✨ Features

- System tray integration
- Start / Stop simulation
- Randomized delay intervals (50–90 seconds)
- Small mouse movement (1px offset)
- Simulated `Shift` key press
- CancellationToken-based background task
- Event-driven state notification

---

## ⚙ How It Works

The simulator runs a background loop that:

1. Slightly moves the mouse (1px right/left)
2. Simulates a short `Shift` key press
3. Waits for a random interval (50–90 seconds)
4. Repeats until stopped

It uses:

- `SendInput` for mouse movement
- `keybd_event` for keyboard simulation
- `CancellationToken` for safe stopping

---

## 🧠 Core Implementation

```csharp
private async Task RunSimulationAsync(CancellationToken token)
{
    var random = new Random();

    while (!token.IsCancellationRequested)
    {
        var pos = Cursor.Position;

        for (int i = 0; i < 5; i++)
        {
            if (token.IsCancellationRequested)
                return;

            MoveMouse(1, 0);
            MoveMouse(-1, 0);

            await Task.Delay(1000, token);

            SimulateShiftKeyPress();
        }

        int delaySeconds = random.Next(50, 91);
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds), token);

        if (token.IsCancellationRequested)
            break;

        SimulateShiftKeyPress();
        MoveMouse(1, 0);
        MoveMouse(-1, 0);
    }
}
```

---

## Mouse Movement (SendInput)

```csharp
private void MoveMouse(int dx, int dy)
{
    INPUT[] input = new INPUT[1];

    input[0].type = INPUT_MOUSE;
    input[0].mi.dx = dx;
    input[0].mi.dy = dy;
    input[0].mi.dwFlags = MOUSEEVENTF_MOVE;

    SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
}
```

---

## Keyboard Simulation (Shift Key)

```csharp
private void SimulateShiftKeyPress()
{
    keybd_event(VK_SHIFT, 0, 0, UIntPtr.Zero);
    Thread.Sleep(50);
    keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
}
```

---

## ⚠ IMPORTANT WARNING

This software simulates user activity at the operating system level.

Using this application may:

- Violate company IT policies  
- Trigger monitoring systems  
- Put your job at risk  
- Potentially breach acceptable-use agreements  

Many organizations actively monitor system behavior, synthetic input, and user presence states.

You are fully responsible for how you use this software.

Use at your own risk.

---

## 🔐 Disclaimer

This project is provided for educational purposes only.

The author assumes no responsibility for misuse, policy violations, disciplinary action, or employment consequences resulting from use of this software.

---

## 🛠 Technologies Used

- .NET WinForms
- P/Invoke (`user32.dll`)
- `SendInput`
- `keybd_event`
- Async / Await
- CancellationToken
- System Tray (`NotifyIcon`)



## 📦 Tech Stack

- C#
- .NET (WinForms)
- Windows API (`user32.dll` → `SetCursorPos`)
- Async / Await pattern
- Event-driven architecture

---



## ▶ How to Run

```bash
git clone https://github.com/raju-melveetilpurayil/TeamsSimulator.git
cd TeamsSimulator
dotnet build
dotnet run
```




---

## ⚖ Legal Disclaimer

This software is provided "as is", without warranty of any kind, express or implied.

The author makes no guarantees regarding the functionality, reliability, or suitability of this application for any specific purpose.

By using this software, you acknowledge and agree that:

- You are solely responsible for how it is used.
- You understand it may violate workplace policies or IT security guidelines.
- You accept all risks associated with its usage.
- The author is not liable for any direct, indirect, incidental, or consequential damages arising from its use.

This project is intended strictly for educational and research purposes related to Windows input simulation and background task handling.

If you are using a work-managed device or corporate account, you are responsible for ensuring compliance with your organization's policies before running this software.
