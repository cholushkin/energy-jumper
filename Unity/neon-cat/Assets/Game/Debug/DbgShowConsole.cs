// using IngameDebugConsole;
//
// public class DbgShowConsole : Pane
// {
//     public DebugLogPopup DebugConsolePopup;
//     public DebugLogManager Console;
//     
//     public override void InitializeState()
//     {
//         base.InitializeState();
//         SetText("<b><color=red>Delete debug tools.</color></b>\nNote that you need to reload the game again to be able to use debug tools again");
//     }
//
//     public override void OnClick()
//     {
//         Console.ShowLogWindow();
//     }
//
//     public override void Update()
//     {
//         SetText($"Console\nInfos: <b>[{DebugConsolePopup.newInfoCount}]</b>\nWarnings: <b>[{DebugConsolePopup.newWarningCount}]</b>\nErrors: <b>[{DebugConsolePopup.newErrorCount}]</b>");
//     }
// }
