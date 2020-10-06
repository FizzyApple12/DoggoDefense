public class AnnouncementData {
    public string message;
    public int delay;
    public bool unpause;
    public AnnouncementData(string arg1, int arg2, bool arg3) {
        message = arg1;
        delay = arg2;
        unpause = arg3;
    }
}