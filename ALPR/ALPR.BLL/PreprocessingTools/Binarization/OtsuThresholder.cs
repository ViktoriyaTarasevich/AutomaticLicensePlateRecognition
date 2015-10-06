using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALPR.BLL.PreprocessingTools.Binarization
{
    public class OtsuThresholder
    {
	    private readonly int[] _histData;
	    private int _maxLevelValue;
	    private int _threshold;

	    public OtsuThresholder()
	    {
		    _histData = new int[256];
	    }

	    public int[] GetHistData()
	    {
		    return _histData;
	    }

	    public int GetMaxLevelValue()
	    {
		    return _maxLevelValue;
	    }

	    public int GetThreshold()
	    {
		    return _threshold;
	    }

	    public int DoThreshold(byte[] srcData,ref byte[] monoData)
	    {
		    int ptr;

		    // Clear histogram data
		    // Set all values to zero
		    ptr = 0;
		    while (ptr < _histData.Length) _histData[ptr++] = 0;

		    // Calculate histogram and find the level with the max value
		    // Note: the max level value isn't required by the Otsu method
		    ptr = 0;
		    _maxLevelValue = 0;
		    while (ptr < srcData.Length)
		    {
			    int h = 0xFF & srcData[ptr];
			    _histData[h] ++;
			    if (_histData[h] > _maxLevelValue) _maxLevelValue = _histData[h];
			    ptr ++;
		    }

		    // Total number of pixels
		    int total = srcData.Length;

		    float sum = 0;
		    for (int t=0 ; t<256 ; t++) sum += t * _histData[t];

		    float sumB = 0;
		    int wB = 0;
		    int wF = 0;

		    float varMax = 0;
		    _threshold = 0;

		    for (int t=0 ; t<256 ; t++)
		    {
			    wB += _histData[t];					// Weight Background
			    if (wB == 0) continue;

			    wF = total - wB;						// Weight Foreground
			    if (wF == 0) break;

			    sumB += (float) (t * _histData[t]);

			    float mB = sumB / wB;				// Mean Background
			    float mF = (sum - sumB) / wF;		// Mean Foreground

			    // Calculate Between Class Variance
			    float varBetween = (float)wB * (float)wF * (mB - mF) * (mB - mF);	

			    // Check if new maximum found
			    if (varBetween > varMax) {
				    varMax = varBetween;
				    _threshold = t;
			    }
		    }

		    // Apply threshold to create binary image
		    if (monoData != null)
		    {
			    ptr = 0;
			    while (ptr < srcData.Length)
			    {
				    monoData[ptr] = (byte) (((0xFF & srcData[ptr]) >= _threshold) ? (byte) 255 : 0);
				    ptr ++;
			    }
		    }

		    return _threshold;
	    }

        
    }

}
