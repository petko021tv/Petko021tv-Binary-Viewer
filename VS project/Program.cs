using Raylib_cs;

namespace HexDisplayer
{
    class Program
    {
        private const int DISPLAY_WIDTH = 1024;
        private const int DISPLAY_HEIGHT = 1024;
        private const int STATS_OFFSET = 330;
        public static unsafe void Main(string[] args)
        {
            Raylib.InitWindow(DISPLAY_WIDTH + STATS_OFFSET, DISPLAY_HEIGHT, "Petko021tv's Binary Drawer");
            Raylib.SetTargetFPS(30);
            Start(args);
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(SKY_BLUE);
                Update();
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }

        //Default paths if nothing is specified in args[]
        //Used for code debugging, feel free to change what you wish.
        private static string[] DefaultPaths = new string[2]
        {
            "D:\\TFH modding\\Modified\\characters-foits\\temp\\tianhuo_charSelect.foit",
            "D:\\TFH modding\\Modified\\characters-foits\\temp\\tianhuo.foit"
        };
        private static string[] FilePaths = new string[0];
        private static string[] FileNames = new string[0];
        private static byte[][] FileDatas = new byte[0][];
        private static int[] CameraPositions = new int[0];
        private static int[] OffsetPositions = new int[0];
        private static int DisplayWidth = 128;
        private static int DisplaySize = 4;
        private static int FileDisplayedIndex = 0;
        private static Color[] ByteColors = new Color[256];
        private static bool HasAnyFilesToDisplay = false;

        private static void Start(string[] args)
        {
            for (int i = 0; i < 256; i++)
                ByteColors[i] = new Color(i, i, i, 255);

            LoadFiles(args);
        }
        private static void Update()
        {
            if (!HasAnyFilesToDisplay)
            {
                Raylib.DrawText("No valid files found to display!", 10, 40, 30, BLACK);
                Raylib.DrawText("Drag suitable files onto exe!", 10, 75, 30, BLACK);
                return;
            }

            ProcessKeyboardInput();

            DisplayStats();

            DisplayFileData();
        }
        private static void LoadFiles(string[] args)
        {
            if (TryLoadValidFiles(args))
            {
                HasAnyFilesToDisplay = true;
            }
            else
            {
                HasAnyFilesToDisplay = TryLoadValidFiles(DefaultPaths);
            }
            if (!HasAnyFilesToDisplay)
                return;

            FileDatas = new byte[FilePaths.Length][];
            for (int i = 0; i < FilePaths.Length; i++)
            {
                FileDatas[i] = LoadFileData(FilePaths[i]);
            }
        }
        private static byte[] LoadFileData(string filePath)
        {
            if (!File.Exists(filePath))
                return Array.Empty<byte>();
            try
            {
                using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using BinaryReader br = new BinaryReader(fs);
                return br.ReadBytes((int)fs.Length);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }
        private static int GetExistingFilePathCount(string[] filePaths)
        {
            int existingFileCount = 0;
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (File.Exists(filePaths[i]))
                    existingFileCount++;
            }
            return existingFileCount;
        }
        private static bool TryLoadValidFiles(string[] paths)
        {
            int validFileCount = GetExistingFilePathCount(paths);
            CameraPositions = new int[validFileCount];
            OffsetPositions = new int[validFileCount];
            FilePaths = new string[validFileCount];
            FileNames = new string[validFileCount];
            int workingListCounter = 0;
            for (int i = 0; i < paths.Length; i++)
            {
                if (!File.Exists(paths[i]))
                    continue;

                FilePaths[workingListCounter] = paths[i];
                FileNames[workingListCounter] = Path.GetFileName(paths[i]);
                workingListCounter++;
            }

            return validFileCount > 0;
        }
        private static void ProcessKeyboardInput()
        {
            bool isShift = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

            if (Raylib.IsKeyPressed(KeyboardKey.F))
            {
                FileDisplayedIndex++;
                if (FileDisplayedIndex >= FilePaths.Length)
                    FileDisplayedIndex = 0;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Comma))
            {
                OffsetPositions[FileDisplayedIndex] += isShift ? 8 : 1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Period))
            {
                OffsetPositions[FileDisplayedIndex] -= isShift ? 8 : 1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))
            {
                CameraPositions[FileDisplayedIndex] -= isShift ? DisplayWidth * 32 : DisplayWidth * 4;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))
            {
                CameraPositions[FileDisplayedIndex] += isShift ? DisplayWidth * 32 : DisplayWidth * 4;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A))
            {
                if (isShift)
                    DisplayWidth /= 2;
                else
                    DisplayWidth--;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D))
            {
                if (isShift)
                    DisplayWidth *= 2;
                else
                    DisplayWidth++;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.One))
            {
                DisplaySize = 1;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Two))
            {
                DisplaySize = 2;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Three))
            {
                DisplaySize = 4;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Four))
            {
                DisplaySize = 8;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Five))
            {
                DisplaySize = 16;
            }
            CameraPositions[FileDisplayedIndex] = Math.Clamp(CameraPositions[FileDisplayedIndex] - (CameraPositions[FileDisplayedIndex] % DisplayWidth), 0, FileDatas[FileDisplayedIndex].Length);
            DisplayWidth = Math.Clamp(DisplayWidth, 1, DISPLAY_WIDTH / DisplaySize);
        }
        private static void DisplayStats()
        {
            Raylib.DrawText("Binary Viewer", DISPLAY_WIDTH, 10, 20, BLACK);
            Raylib.DrawText("By Petko021tv", DISPLAY_WIDTH, 30, 20, BLACK);
            Raylib.DrawFPS(DISPLAY_WIDTH, 60);
            Raylib.DrawText("File: " + FileNames[FileDisplayedIndex], DISPLAY_WIDTH, 150, 20, BLACK);
            Raylib.DrawText("Camera Position: " + CameraPositions[FileDisplayedIndex], DISPLAY_WIDTH, 180, 20, BLACK);
            Raylib.DrawText("Display Witdth: " + DisplayWidth, DISPLAY_WIDTH, 210, 20, BLACK);
            Raylib.DrawText("Display Offset: " + OffsetPositions[FileDisplayedIndex], DISPLAY_WIDTH, 240, 20, BLACK);
            Raylib.DrawText("Zoom: " + DisplaySize + "x", DISPLAY_WIDTH, 270, 20, BLACK);
            
            Raylib.DrawText("---Controls---", DISPLAY_WIDTH, 400, 20, BLACK);
            Raylib.DrawText("Up: W / Up Arrow", DISPLAY_WIDTH, 430, 20, BLACK);
            Raylib.DrawText("Down: S / Down Arrow", DISPLAY_WIDTH, 460, 20, BLACK);
            Raylib.DrawText("Widen: D / Right Arrow", DISPLAY_WIDTH, 490, 20, BLACK);
            Raylib.DrawText("Narrow: A / Left Arrow", DISPLAY_WIDTH, 520, 20, BLACK);
            Raylib.DrawText("Offset Right: .", DISPLAY_WIDTH, 550, 20, BLACK);
            Raylib.DrawText("Offset Left: ,", DISPLAY_WIDTH, 580, 20, BLACK);
            Raylib.DrawText("Zoom: 1,2,3,4,5", DISPLAY_WIDTH, 610, 20, BLACK);
            Raylib.DrawText("Switch File: F", DISPLAY_WIDTH, 640, 20, BLACK);
            Raylib.DrawText("Shift can amplify actions", DISPLAY_WIDTH, 670, 20, BLACK);
            //Raylib.DrawText("Debug  " + FileDatas[0].Length, WINDOW_WIDTH - STATS_OFFSET, 190, 20, Color.BLACK);
        }
        private static void DisplayFileData()
        {
            int position = CameraPositions[FileDisplayedIndex];
            int offset = OffsetPositions[FileDisplayedIndex];
            for (int i = Math.Clamp(position - offset, 0, FileDatas[FileDisplayedIndex].Length);
                  i < Math.Clamp((position + (DISPLAY_HEIGHT * DisplayWidth)) + offset, 0, FileDatas[FileDisplayedIndex].Length); i++)
                Raylib.DrawRectangle(((i + offset) % DisplayWidth) * DisplaySize,
                    (i + offset - position) / DisplayWidth * DisplaySize,
                    DisplaySize, DisplaySize, ByteColors[FileDatas[FileDisplayedIndex][i]]);
        }

        private static Color SKY_BLUE = new(102, 191, 255, 255);
        private static Color BLACK = new(0, 0, 0, 255);
    }
}

