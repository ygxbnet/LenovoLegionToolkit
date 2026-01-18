using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.CLI;

public class Program
{
    public static Task<int> Main(string[] args) => BuildCommandLine().Parse(args).InvokeAsync();

    private static RootCommand BuildCommandLine()
    {
        var root = new RootCommand("Utility that controls Lenovo Legion Toolkit from command line.\n\n" +
                                   "Lenovo Legion Toolkit must be running in the background and CLI setting must be " +
                                   "turned on for this utility to work.");

        root.Add(BuildQuickActionsCommand());
        root.Add(BuildFeatureCommand());
        root.Add(BuildSpectrumCommand());
        root.Add(BuildRGBCommand());

        return root;
    }

    private static Command BuildQuickActionsCommand()
    {
        var nameArgument = new Argument<string>( "Name of the Quick Action") { Arity = ArgumentArity.ZeroOrOne };

        var listOption = new Option<bool>("--list", "List available Quick Actions") { Arity = ArgumentArity.ZeroOrOne };
        listOption.Aliases.Add("-l");

        var cmd = new Command("quickAction", "Run Quick Action");
        cmd.Aliases.Add("qa");
        cmd.Arguments.Add(nameArgument);
        cmd.Options.Add(listOption);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var name = parseResult.GetValue(nameArgument);
            var list = parseResult.GetValue(listOption);

            if (list)
            {
                var result = await IpcClient.ListQuickActionsAsync();
                Console.WriteLine(result);
                return 0;
            }

            await IpcClient.RunQuickActionAsync(name);
            return 0;
        });
        cmd.Validators.Add(result =>
        {
            if (result.GetResult(nameArgument) is not null)
                return;

            if (result.GetResult(listOption) is not null)
                return;

            result.AddError($"{nameArgument.Name} or --{listOption.Name} should be specified");
        });

        return cmd;
    }

    private static Command BuildFeatureCommand()
    {
        var getCmd = BuildGetFeatureCommand();
        var setCmd = BuildSetFeatureCommand();

        var listOption = new Option<bool?>("--list", "List available features") { Arity = ArgumentArity.ZeroOrOne };
        listOption.Aliases.Add("-l");

        var cmd = new Command("feature", "Control features");
        cmd.Aliases.Add("f");
        cmd.Add(getCmd);
        cmd.Add(setCmd);
        cmd.Options.Add(listOption);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var list = parseResult.GetValue(listOption);

            if (!list.HasValue || !list.Value)
                return 0;

            var value = await IpcClient.ListFeaturesAsync();
            Console.WriteLine(value);
            return 0;
        });
        cmd.Validators.Add(result =>
        {
            if (result.GetResult(getCmd) is not null)
                return;

            if (result.GetResult(setCmd) is not null)
                return;

            if (result.GetResult(listOption) is not null)
                return;

            result.AddError($"{getCmd.Name}, {setCmd.Name} or --{listOption.Name} should be specified");
        });

        return cmd;
    }

    private static Command BuildGetFeatureCommand()
    {
        var nameArgument = new Argument<string>( "Name of the feature") { Arity = ArgumentArity.ExactlyOne };

        var cmd = new Command("get", "Get value of a feature");
        cmd.Aliases.Add("g");
        cmd.Arguments.Add(nameArgument);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var name = parseResult.GetValue(nameArgument);
            var result = await IpcClient.GetFeatureValueAsync(name);
            Console.WriteLine(result);
            return 0;
        });

        return cmd;
    }

    private static Command BuildSetFeatureCommand()
    {
        var nameArgument = new Argument<string>( "Name of the feature") { Arity = ArgumentArity.ExactlyOne };
        var valueArgument = new Argument<string>( "Value of the feature") { Arity = ArgumentArity.ZeroOrOne };

        var listOption = new Option<bool>("--list", "List available feature values") { Arity = ArgumentArity.ZeroOrOne };
        listOption.Aliases.Add("-l");

        var cmd = new Command("set", "Set value of a feature");
        cmd.Aliases.Add("s");
        cmd.Arguments.Add(nameArgument);
        cmd.Arguments.Add(valueArgument);
        cmd.Options.Add(listOption);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var name = parseResult.GetValue(nameArgument);
            var value = parseResult.GetValue(valueArgument);
            var list = parseResult.GetValue(listOption);

            if (list)
            {
                var result = await IpcClient.ListFeatureValuesAsync(name);
                Console.WriteLine(result);
                return 0;
            }

            await IpcClient.SetFeatureValueAsync(name, value);
            return 0;
        });
        cmd.Validators.Add(result =>
        {
            if (result.GetResult(nameArgument) is not null)
                return;

            if (result.GetResult(listOption) is not null)
                return;

            result.AddError($"{nameArgument.Name} or --{listOption.Name} should be specified");
        });

        return cmd;
    }

    private static Command BuildSpectrumCommand()
    {
        var profileCommand = BuildSpectrumProfileCommand();
        var brightnessCommand = BuildSpectrumBrightnessCommand();

        var cmd = new Command("spectrum", "Control Spectrum backlight");
        cmd.Aliases.Add("s");
        cmd.Add(profileCommand);
        cmd.Add(brightnessCommand);
        return cmd;
    }

    private static Command BuildSpectrumProfileCommand()
    {
        var getCmd = BuildGetSpectrumProfileCommand();
        var setCmd = BuildSetSpectrumProfileCommand();

        var cmd = new Command("profile", "Control Spectrum backlight profile");
        cmd.Aliases.Add("p");
        cmd.Add(getCmd);
        cmd.Add(setCmd);

        return cmd;
    }

    private static Command BuildGetSpectrumProfileCommand()
    {
        var cmd = new Command("get", "Get current Spectrum profile");
        cmd.Aliases.Add("g");
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var result = await IpcClient.GetSpectrumProfileAsync();
            Console.WriteLine(result);
            return 0;
        });

        return cmd;
    }

    private static Command BuildSetSpectrumProfileCommand()
    {
        var valueArgument = new Argument<int>( "Profile to set") { Arity = ArgumentArity.ExactlyOne };

        var cmd = new Command("set", "Set current Spectrum profile");
        cmd.Aliases.Add("s");
        cmd.Arguments.Add(valueArgument);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var value = parseResult.GetValue(valueArgument);
            await IpcClient.SetSpectrumProfileAsync($"{value}");
            return 0;
        });

        return cmd;
    }

    private static Command BuildSpectrumBrightnessCommand()
    {
        var getCmd = BuildGetSpectrumBrightnessCommand();
        var setCmd = BuildSetSpectrumBrightnessCommand();

        var cmd = new Command("brightness", "Control Spectrum brightness");
        cmd.Aliases.Add("b");
        cmd.Add(getCmd);
        cmd.Add(setCmd);

        return cmd;
    }

    private static Command BuildGetSpectrumBrightnessCommand()
    {
        var cmd = new Command("get", "Get current Spectrum brightness");
        cmd.Aliases.Add("g");
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var result = await IpcClient.GetSpectrumBrightnessAsync();
            Console.WriteLine(result);
            return 0;
        });

        return cmd;
    }

    private static Command BuildSetSpectrumBrightnessCommand()
    {
        var valueArgument = new Argument<int>( "Brightness to set") { Arity = ArgumentArity.ExactlyOne };

        var cmd = new Command("set", "Set current Spectrum brightness");
        cmd.Aliases.Add("s");
        cmd.Arguments.Add(valueArgument);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var value = parseResult.GetValue(valueArgument);
            await IpcClient.SetSpectrumBrightnessAsync($"{value}");
            return 0;
        });

        return cmd;
    }

    private static Command BuildRGBCommand()
    {
        var getCmd = BuildGetRGBCommand();
        var setCmd = BuildSetRGBCommand();

        var cmd = new Command("rgb", "Control RGB backlight preset");
        cmd.Aliases.Add("r");
        cmd.Add(getCmd);
        cmd.Add(setCmd);

        return cmd;
    }

    private static Command BuildGetRGBCommand()
    {
        var cmd = new Command("get", "Get current RGB preset");
        cmd.Aliases.Add("g");
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var result = await IpcClient.GetRGBPresetAsync();
            Console.WriteLine(result);
            return 0;
        });

        return cmd;
    }

    private static Command BuildSetRGBCommand()
    {
        var valueArgument = new Argument<int>( "Preset to set") { Arity = ArgumentArity.ExactlyOne };

        var cmd = new Command("set", "Set current RGB preset");
        cmd.Aliases.Add("s");
        cmd.Arguments.Add(valueArgument);
        cmd.SetAction(async (ParseResult parseResult, CancellationToken token) =>
        {
            var value = parseResult.GetValue(valueArgument);
            await IpcClient.SetRGBPresetAsync($"{value}");
            return 0;
        });

        return cmd;
    }

    // private static void OnException(Exception ex, InvocationContext context)
    // {
    //     var message = ex switch
    //     {
    //         IpcConnectException => "Failed to connect. " +
    //                                "Make sure that Lenovo Legion Toolkit is running " +
    //                                "in background and CLI is enabled in Settings.",
    //         IpcException => ex.Message,
    //         _ => ex.ToString()
    //     };
    //     var exitCode = ex switch
    //     {
    //         IpcConnectException => -1,
    //         IpcException => -2,
    //         _ => -99
    //     };
    //
    //     if (!Console.IsOutputRedirected)
    //     {
    //         Console.ResetColor();
    //         Console.ForegroundColor = ConsoleColor.Red;
    //     }
    //
    //     context.Console.Error.WriteLine(message);
    //     context.ExitCode = exitCode;
    //
    //     if (!Console.IsOutputRedirected)
    //         Console.ResetColor();
    // }
}
