
using System.Xml.Schema;

namespace CEF;
public static class Undo
{


    //Undo infra
    public static int max_back_bd = 100;
    private static string[] undoShadows = new string[max_back_bd];
    private static int undoSeek = 0;
    private static bool undoCapturePaused = false;

    private static ModelDB model;

    public static void undoSetUp(ModelDB Model)
    {
        model = Model;
    }

    public static void pushUndo(in string item)
    {
        if (string.IsNullOrEmpty(item)) return;
        undoSeek++;
        if (undoSeek >= max_back_bd)
            undoSeek = 0;
        undoShadows[undoSeek] = item;
    }

    public static string popUndo()
    {
        var ret = undoShadows[undoSeek];
        if (string.IsNullOrEmpty(ret)|| ret == "") return string.Empty;        
        undoShadows[undoSeek] = string.Empty;
        undoSeek--;
        if (undoSeek < 0) 
            undoSeek = (max_back_bd-1);
        return ret;
    }

    public static void restartUndo()
    {
        undoSeek = 0;
        undoShadows = new string[max_back_bd];
    }


    public static  void stopUndoRecording() { undoCapturePaused = true; }
    public static  void startUndoRecording() { undoCapturePaused = false; }

    public static void takeUndoSnapShot()
    {
        if (!undoCapturePaused)
        {
            pushUndo(model.SaveToStream());
        }
    }
    //End Undo infra
}

   
