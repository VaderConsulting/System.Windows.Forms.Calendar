# System.Windows.Forms.Calendar

System.Windows.Forms.Calendar is Jose Menendez Póo's C# WinForms Outlook-style calendar with appointments, day views, and MonthView. The `Calendar` control shows a date range in Expanded (column) or Short (week-row) mode, lays out overlapping items, and supports drag, resize, in-place text edit, `ApplyColor` shading, and a configurable time scale; `MonthView` is an Outlook-style month picker that sizes to its container instead of forcing a fixed layout. CalendarDemo hosts both controls, persists items to `items.xml`, and exposes context-menu timescale and color commands. This tree is Dave Robinson's Historical Dev working copy: the library was retargeted to .NET Framework 3.5 (`System.Windows.Forms.Calendar35.sln`) with `CalendarTimeScaleFormat` and `IScrollableControl`; `Original/` keeps the .NET 2.0 CodeProject tree.

**Source last updated:** 2013-08-28 · **Language:** C# · **Target:** .NET Framework 3.5 (Visual Studio 2012; Original is .NET 2.0 / ProductVersion 8.0.50727) · **Output:** class library + WinForms demo exe

## Solution structure

| Project | Language | Type | Purpose |
|---------|----------|------|---------|
| `System.Windows.Forms.Calendar` | C# | WinForms library (.NET 3.5) | `Calendar` and `MonthView` controls, renderers, items, and day/week model |
| `CalendarDemo` | C# | WinForms WinExe (.NET 3.5) | DemoForm host: load/save `items.xml`, create/drag/color items |
| `Original/System.Windows.Forms.Calendar` | C# | WinForms library (.NET 2.0) | CodeProject tree before the 3.5 retarget (not in the 35.sln) |
| `Original/CalendarDemo` | C# | WinForms WinExe (.NET 2.0) | Original demo paired with the .NET 2.0 library |

## How to open

Open `System.Windows.Forms.Calendar35.sln` in Visual Studio 2012 or later with .NET Framework 3.5 targeting. Set CalendarDemo as the startup project. The pre-upgrade solution is `Original/System.Windows.Forms.Calendar.sln` (.NET 2.0). `Backup/` holds Visual Studio upgrade copies.

## Attribution and provenance

Working copy from Dave Robinson's OneDrive Historical Dev folder `System.Windows.Forms.Calendar`. Original author **Jose Menendez Póo**, CodeProject article [A Professional Calendar/Agenda View That You Will Use](https://www.codeproject.com/Articles/38699/A-Professional-Calendar-Agenda-View-That-You-Will) (also [WinFormsCalendarView](https://www.codeproject.com/KB/docview/WinFormsCalendarView.aspx)). AssemblyTitle `System.Windows.Forms.Calendar`; AssemblyCopyright Copyright © 2009; AssemblyCompany empty; library AssemblyVersion 3.5.* (Original 1.0.*). Solution Format Version 12.00 (Visual Studio 2012). This is not original VaderConsulting code.

## License

Original license terms apply (Code Project Open License / CPOL on the CodeProject article). This repository does not claim authorship and does not relicense the control as MIT. See `THIRD_PARTY_NOTICES.md`.
