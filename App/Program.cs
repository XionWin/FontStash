namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (GLWindow glWin = new GLWindow("OpenTK-FontStash", 512, 512))
            {
                glWin.Run();
            }
        }
    }
}