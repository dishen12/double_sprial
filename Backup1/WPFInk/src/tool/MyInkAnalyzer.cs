using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;
using WPFInk.Global;

namespace WPFInk.tool
{
	public class MyInkAnalyzer
	{
		InkAnalyzer analyze;
		private string analyzeResults = "";
		private StrokeCollection _strokeCollection;

		public MyInkAnalyzer(StrokeCollection strokeCollection)
		{
			if (strokeCollection.Count>0)
			{
				_strokeCollection = strokeCollection;
				analyze = new InkAnalyzer();
				//2052：简体中文 1033：英语
				analyze.AddStrokes(strokeCollection,GlobalValues.InkAnalyzerLanguageId);
				//analyze.BackgroundAnalyze();

				AnalysisStatus status = analyze.Analyze();

				if (status.Successful)
				{
					analyzeResults = analyze.GetRecognizedString();
				}
				else
				{
					analyzeResults = "Recognition Failed";
				}

				analyze.RemoveStrokes(_strokeCollection);
			}
		}
		
		public string getAnalyzeResults()
        {
			return analyzeResults;
		}

	}
}
