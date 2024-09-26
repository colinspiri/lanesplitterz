#ifndef EROSIONOUTLINES_INCLUDED
#define EROSIONOUTLINES_INCLUDED

void CelShading_float(float luminance, out float Out)
{
    // luminance = saturate(luminance);
    if (luminance > 0.0f)
        luminance = 0.3f;
    else if (luminance > 0.5f)
        luminance = 0.7f;
    else
        luminance = 1.0f;
    Out = luminance;
}
#endif