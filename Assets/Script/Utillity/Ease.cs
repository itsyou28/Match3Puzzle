using UnityEngine;

// easing demo site link http://robertpenner.com/easing/easing_demo.html
public static class Ease
{
    private const float Pi = 3.14159265f;

    public static float InQuad(float v)
    {
        return v * v;
    }

    public static float OutQuad(float v)
    {
        return -v * (v - 2.0f);
    }

    public static float InOutQuad(float v)
    {
        float t = v * 2;

        if (t < 1)
            return 0.5f * t * t;

        t -= 1;

        return -0.5f * (t * (t - 2.0f) - 1.0f);
    }

    public static float InCubic(float v)
    {
        return v * v * v;
    }

    public static float OutCubic(float v)
    {
        float t = v - 1.0f;

        return t * t * t + 1.0f;
    }

    public static float InOutCubic(float v)
    {
        float t = v * 2.0f;

        if (t < 1)
            return 0.5f * t * t * t;

        t -= 2.0f;

        return 0.5f * (t * t * t + 2.0f);
    }

    public static float InQuartic(float v)
    {
        return v * v * v * v;
    }

    public static float OutQuartic(float v)
    {
        float t = v - 1.0f;

        return -(t * t * t * t - 1.0f);
    }

    public static float InOutQuartic(float v)
    {
        float t = v * 2.0f;

        if (t < 1)
            return 0.5f * t * t * t * t;

        t -= 2.0f;

        return -0.5f * (t * t * t * t - 2.0f);
    }

    public static float InQuintic(float v)
    {
        return v * v * v * v * v;
    }

    public static float OutQuintic(float v)
    {
        float t = v - 1.0f;

        return t * t * t * t * t + 1;
    }

    public static float InOutQuintic(float v)
    {
        float t = v * 2.0f;

        if (t < 1.0f)
            return 0.5f * t * t * t * t * t;

        t -= 2.0f;

        return 0.5f * (t * t * t * t * t + 2.0f);
    }

    public static float InSine(float v)
    {
        return -Mathf.Cos(v * Pi / 2.0f) + 1.0f;
    }

    public static float OutSine(float v)
    {
        return Mathf.Sin(v * Pi / 2.0f);
    }

    public static float InOutSine(float v)
    {
        return -0.5f * (Mathf.Cos(v * Pi) - 1.0f);
    }

    public static float InExpo(float v)
    {
        if (v == 0.0f)
            return 0.0f;

        return Mathf.Pow(2.0f, 10.0f * (v - 1.0f));
    }

    public static float OutExpo(float v)
    {
        if (v == 1.0f)
            return 1.0f;

        return -Mathf.Pow(2.0f, -10.0f * v) + 1.0f;
    }

    public static float InOutExpo(float v)
    {
        if (v == 0.0f)
            return 0.0f;
        else if (v == 1.0f)
            return 1.0f;

        float t = v * 2;

        if (t < 1)
            return 0.5f * Mathf.Pow(2.0f, 10.0f * (t - 1.0f));

        return 0.5f * (-Mathf.Pow(2.0f, -10.0f * (t - 1.0f)) + 2.0f);
    }

    public static float InCirc(float v)
    {
        if (v <= 0.0f)
            return 0.0f;
        else if (v >= 1.0f)
            return 1.0f;

        return -(Mathf.Sqrt(1.0f - v * v) - 1.0f);
    }

    public static float OutCirc(float v)
    {
        if (v <= 0.0f)
            return 0.0f;
        else if (v >= 1.0f)
            return 1.0f;

        float t = v - 1.0f;

        return Mathf.Sqrt(1.0f - t * t);
    }

    public static float InOutCirc(float v)
    {
        if (v <= 0.0f)
            return 0.0f;
        else if (v >= 1.0f)
            return 1.0f;

        float t = v * 2.0f;

        if (t < 1.0f)
            return -0.5f * (Mathf.Sqrt(1.0f - t * t) - 1.0f);

        t -= 2.0f;

        return 0.5f * (Mathf.Sqrt(1.0f - t * t) + 1.0f);
    }

    public static float InElastic(float v)
    {
        if (v == 0.0f)
            return 0.0f;
        else if (v == 1.0f)
            return 1.0f;

        float p = 0.3f;
        float s = p / 4.0f;
        float t = v - 1.0f;

        return -(Mathf.Pow(2.0f, 10.0f * t)) * Mathf.Sin((t - s) * (2.0f * Pi) / p);
    }

    public static float OutElastic(float v)
    {
        if (v == 0.0f)
            return 0.0f;
        else if (v == 1.0f)
            return 1.0f;

        float p = 0.3f;
        float s = p / 4.0f;

        return Mathf.Pow(2.0f, -10.0f * v) * Mathf.Sin((v - s) * (2.0f * Pi) / p) + 1.0f;
    }

    public static float InOutElastic(float v)
    {
        if (v == 0.0f)
            return 0.0f;
        else if (v == 1.0f)
            return 1.0f;

        float p = 0.3f;
        float s = p / 4.0f;
        float t = v * 2.0f - 1.0f;

        if (t < 0.0f)
            return -0.5f * (Mathf.Pow(2.0f, 10.0f * t) * Mathf.Sin((t - s) * (2.0f * Pi) / p));

        return Mathf.Pow(2.0f, -10.0f * t) * Mathf.Sin((t - s) * (2.0f * Pi) / p) * 0.5f + 1.0f;
    }

    public static float InBack(float v)
    {
        float s = 1.70158f;

        return v * v * ((s + 1.0f) * v - s);
    }

    public static float OutBack(float v)
    {
        float s = 1.70158f;
        float t = v - 1;

        return t * t * ((s + 1.0f) * t + s) + 1.0f;
    }

    public static float InOutBack(float v)
    {
        float s = 1.70158f;
        float t = v * 2.0f;

        s *= 1.525f;

        if (t < 1.0f)
            return 0.5f * (t * t * ((s + 1.0f) * t - s));

        t -= 2.0f;

        return 0.5f * (t * t * ((s + 1.0f) * t + s) + 2.0f);
    }

    public static float InBounce(float v)
    {
        return 1.0f - OutBounce(1 - v);
    }

    public static float OutBounce(float v)
    {
        float t = v;

        if (t < 1.0f / 2.75f)
        {
            return 7.5625f * t * t;
        }
        else if (t < 2.0f / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < 2.5f / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }

    public static float InOutBounce(float v)
    {
        if (v < 0.5f)
            return 0.5f - OutBounce(1.0f - v * 2.0f) * 0.5f;
        else
            return 0.5f + OutBounce(v * 2.0f - 1.0f) * 0.5f;
    }
    
}