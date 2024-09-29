#ifndef EROSIONOUTLINES_INCLUDED
#define EROSIONOUTLINES_INCLUDED

void CelShading_float(float luminance, out float Out)
{
    Out = luminance;
    if (luminance >= 0.0f && luminance < 0.2f)
        Out = 0.0f;
    else if (luminance >= 0.2f && luminance < 0.4f)
        Out = 0.2f;
    else if (luminance >= 0.4f && luminance < 0.6f)
        Out = 0.4f;
    else if (luminance >= 0.6f && luminance < 0.8f)
        Out = 0.6f;
    else if (luminance >= 0.8f && luminance < 1.0f)
        Out = 0.8f;
    else
        Out = 1.0f;
}
#endif