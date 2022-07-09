using System.Drawing;
using System.Drawing.Imaging;

namespace DemoApp;

public class Program
{
    readonly static string _inputDirectory = "..\\..\\..\\inputPhotos\\";
    readonly static string _outputDirectory = "..\\..\\..\\outputPhotos\\";

    readonly static string[] _directories = new[] { _inputDirectory, _outputDirectory };

    static void Main(string[] args)
    {
        // TODO: Extract CreateDirectories, DeleteOutputFolderFiles into a FileService class:
        CreateDirectories();
        DeleteOutputFolderFiles();

        string? fileName = GetFirstInputJpgFile();
        if (fileName is null)
        {
            TerminateProgram($"No jpg files were found in folder '{_inputDirectory}'");
            return;
        }
        Console.WriteLine($"Found file: '{fileName}'");

        (int width, int height) = SaveBmpImage(fileName);
        Console.WriteLine($"Image width: {width}, height: {height}");
    }

    static void TerminateProgram(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey();
    }

    #region void CreateDirectories()
    /// <summary>
    /// Creates directories if they do not exist
    /// </summary>
    static void CreateDirectories()
    {
        foreach (string directory in _directories)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
    #endregion

    #region void DeleteOutputFolderFiles()
    /// <summary>
    /// Deletes files from the output folder
    /// </summary>
    static void DeleteOutputFolderFiles()
    {
        string[] fileNames = Directory.GetFiles(_outputDirectory);
        foreach (string fileName in fileNames)
        {
            File.Delete(fileName);
        }
    }
    #endregion

    #region string? GetFirstInputJpgFile()
    /// <summary>
    /// Looks for jpg files in the input directory
    /// </summary>
    /// <returns>File name if it exists, null otherwise</returns>
    static string? GetFirstInputJpgFile()
    {
        string[] jpgFileNames = Directory.GetFiles(_inputDirectory, "*.jpg");
        return jpgFileNames.Length switch
        {
            > 0 => jpgFileNames[0],
            _ => null
        };
    }
    #endregion

    #region (int Width, int Height) SaveBmpImage(string fileName)
    /// <summary>
    /// Saves jpg image as bmp in the output directory.
    /// </summary>
    /// <param name="fileName">Jpg file name</param>
    /// <returns>Image's width and height in a tuple</returns>
    static (int Width, int Height) SaveBmpImage(string fileName)
    {
        Bitmap image = new(fileName);
        string saveImagePath = Path.Combine(_outputDirectory, Path.GetFileNameWithoutExtension(fileName)) + ".bmp";
        image.Save(saveImagePath, ImageFormat.Bmp);
        return (image.Width, image.Height);
    }
    #endregion
}