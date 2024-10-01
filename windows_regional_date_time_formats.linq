<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	//var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(ci => $"{ci.Name}");
	//cultures.Dump();
	PrintExample("ja");

	var output = new List<object>();
	var longDates = new List<string>();

	foreach (var culture in SupportedCultures())
	{
		try
		{
			// Get the properties of an en-US DateTimeFormatInfo object.

			DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo(culture).DateTimeFormat;
			output.Add(new { culture = culture, longDate = dtfi.LongDatePattern });
			longDates.Add(dtfi.LongDatePattern);
			//		Type typ = dtfi.GetType();
			//		PropertyInfo[] props = typ.GetProperties();
			//		DateTime value = new DateTime(2012, 5, 28, 11, 35, 0);
			//
			//		foreach (var prop in props)
			//		{
			//			// Is this a format pattern-related property?
			//			if (prop.Name.Contains("Pattern"))
			//			{
			//				string fmt = prop.GetValue(dtfi, null).ToString();
			//				Console.WriteLine("{0,-33} {1} \n{2,-37}Example: {3}\n",
			//								  prop.Name + ":", fmt, "",
			//								  value.ToString(fmt));
			//			}
			//		}
		}
		catch (Exception e)
		{
			output.Add(new { culture = culture, error = "⚠ Culture not supported ⚠" });
			Console.WriteLine($"⚠ Culture {culture} not supported");
		}
	}
	output.Dump();
	//string.Join(", ", longDates.Select(d => $"\"{d}\"")).Dump();
}

void PrintExample(string culture)
{
	Console.WriteLine($"Example for {culture}");
	DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo(culture).DateTimeFormat;
	//dtfi.LongDatePattern.Dump();

	Type typ = dtfi.GetType();
	PropertyInfo[] props = typ.GetProperties();
	DateTime value = new DateTime(2012, 5, 28, 11, 35, 0);

	foreach (var prop in props)
	{
		// Is this a format pattern-related property?
		if (prop.Name.Contains("Pattern"))
		{
			string fmt = prop.GetValue(dtfi, null).ToString();
			Console.WriteLine("{0,-33} {1} \n{2,-37}Example: {3}\n",
							  prop.Name + ":", fmt, "",
							  value.ToString(fmt));
		}
	}
}

IEnumerable<string> SupportedCultures() => CultureInfo.GetCultures(CultureTypes.AllCultures).Select(ci => ci.Name);
