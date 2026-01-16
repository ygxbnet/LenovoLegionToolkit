@echo off

rmdir /s /q .vs
rmdir /s /q _ReSharper.Caches

rmdir /s /q build
rmdir /s /q build_installer

rmdir /s /q src\LenovoLegionToolkit.CLI\bin
rmdir /s /q src\LenovoLegionToolkit.CLI\obj

rmdir /s /q src\LenovoLegionToolkit.Lib\bin
rmdir /s /q src\LenovoLegionToolkit.Lib\obj

rmdir /s /q src\LenovoLegionToolkit.Lib.Automation\bin
rmdir /s /q src\LenovoLegionToolkit.Lib.Automation\obj

rmdir /s /q src\LenovoLegionToolkit.Lib.CLI\bin
rmdir /s /q src\LenovoLegionToolkit.Lib.CLI\obj

rmdir /s /q src\LenovoLegionToolkit.Lib.Macro\bin
rmdir /s /q src\LenovoLegionToolkit.Lib.Macro\obj

rmdir /s /q src\LenovoLegionToolkit.WPF\bin
rmdir /s /q src\LenovoLegionToolkit.WPF\obj

rmdir /s /q src\LenovoLegionToolkit.SpectrumTester\bin
rmdir /s /q src\LenovoLegionToolkit.SpectrumTester\obj
