using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PictureSorter {
  public class Program {
    private static readonly Regex Regex = new Regex( ":" );
    private static readonly string BasePath = @"C:\Users\Nick Huguenard\Desktop\iPhone Photos\";
    private static void Main( string[] args ) {
      var dir = new DirectoryInfo( BasePath );
      var files = dir.EnumerateFiles().Where( file => file.Extension == ".JPG" );
      foreach ( var file in files ) {
        var dateTaken = GetDateTaken( file );
        if ( dateTaken == DateTime.MinValue ) {
          continue;
        }
        MoveFile( file, dateTaken );
        Console.WriteLine( dateTaken );
      }
      Console.Read();
    }

    private static DateTime GetDateTaken( FileInfo file ) {
      try {
        using ( var fs = new FileStream( Path.Combine( BasePath, file.Name ), FileMode.Open, FileAccess.Read ) ) {
          using ( var image = Image.FromStream( fs, false, false ) ) {
            var propItem = image.GetPropertyItem( 36867 );
            var dateTakenString = Regex.Replace( Encoding.UTF8.GetString( propItem.Value ), "-", 2 );
            return DateTime.Parse( dateTakenString );
          }
        }
      }
      catch ( Exception ) {
        return file.LastWriteTime;
      }
    }

    private static void MoveFile( FileInfo file, DateTime dateTaken ) {
      var newBasePath = @"C:\Users\Nick Huguenard\Desktop\TestLocation";
      var newPath = Path.Combine( newBasePath, dateTaken.Year.ToString(), dateTaken.ToString( "MMMM" ) );
      if ( !Directory.Exists( newPath ) ) {
        Directory.CreateDirectory( newPath );
      }

      file.MoveTo( Path.Combine( newPath, file.Name ) );
    }
  }
}
