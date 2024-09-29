#ifndef EROSIONOUTLINES_INCLUDED
#define EROSIONOUTLINES_INCLUDED

void CelShading_float(float luminance, out float Out)
{
    if (luminance >= 0.0f && luminance < 0.25f)
        Out = 0.0f;
    else if (luminance >= 0.25f && luminance < 0.5f)
        Out = 0.25f;
    else if (luminance >= 0.5f && luminance < 0.75f)
        Out = 0.5f;
    else if (luminance >= 0.75f && luminance < 1.0f)
        Out = 0.75f;
    else
        Out = 1.0f;
}
#endif