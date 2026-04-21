namespace Spyro_Editor.Constants
{
    enum Game
    {
        Spyro1,
        Spyro2,
        Spyro3
        // Spyro4 (maybe some day...)
    }

    enum WADSignature
    {
        // uint32 at 0x100
        Spyro1_NTSC = 39424000,
        Spyro2_NTSC = 32681984,
        Spyro3_NTSC = 25331712
    }
}
