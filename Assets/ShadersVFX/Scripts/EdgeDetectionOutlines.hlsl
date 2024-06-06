#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// Sobel effect runs by sampling the texture around a point to see if there are any large changes.
// edge detection is done by comparing the color of the current pixel with the color of the pixels around it.
// higher value means more edges detected.

// the uv points to sample around the current pixel
static float2 sobelSamplePoints[9] = {
    float2(-1, 1), float2(0, 1), float2(1, 1),
    float2(-1, 0), float2(0, 0), float2(1, 0),
    float2(-1, -1), float2(0, -1), float2(1, -1)
};

// the weight matrixes
static float sobelXWeights[9] = {
    1, 0, -1,
    2, 0, -2,
    1, 0, -1
};

static float sobelYWeights[9] = {
    1, 2, 1,
    0, 0, 0,
    -1, -2, -1
};

// runs Sobel over depth texture
void DepthSobel_float(float2 UV, float Thickness, out float Out)
{
    float2 sobel = 0;
    // unroll for more efficiency
    // get depth values
    [unroll] for (int i = 0; i < 9; i++)
    {
        float2 sampleUV = UV + sobelSamplePoints[i] * Thickness;
        float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(sampleUV);
        sobel += depth * float2(sobelXWeights[i], sobelYWeights[i]);
    }
    Out = length(sobel);
}

void ColorSobel_float(float2 UV, float Thickness, out float Out)
{
    float2 sobelRed = 0;
    float2 sobelGreen = 0;
    float2 sobelBlue = 0;
    // unroll for more efficiency
    // get color values
    [unroll] for (int i = 0; i < 9; i++)
    {
        float2 sampleUV = UV + sobelSamplePoints[i] * Thickness;
        float3 color = SHADERGRAPH_SAMPLE_SCENE_COLOR(sampleUV);
        sobelRed += color.r * float2(sobelXWeights[i], sobelYWeights[i]);
        sobelGreen += color.g * float2(sobelXWeights[i], sobelYWeights[i]);
        sobelBlue += color.b * float2(sobelXWeights[i], sobelYWeights[i]);
    }
    Out = max(length(sobelRed), max(length(sobelGreen), length(sobelBlue)));
}
#endif