namespace BarcodeCreator_Framework
{
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Windows.Media;

	public class BarcodeEngine
	{
		public double Code39WideRate { get; set; } = 2.3;
		public CodeTypes CodeType { get; set; } = CodeTypes.Code39;
		public double BarWidth { get; set; } = 1;

		//W Wide - Black
		//N Narrow - Black
		//w Wide - White
		//n Narrow - White

		public List<BarcodeItem> Generate(string barcode)
		{
			switch (CodeType)
			{
				case CodeTypes.Code128A:
					var code128AKeys = barcode.Where(x => Codes.Code.ContainsKey(x.ToString())).Select(x => Codes.Code[x.ToString()]).ToList();
					return GetResult(code128AKeys, 103);

				case CodeTypes.Code128B:
					var code128BKeys = barcode.Where(x => Codes.Code.ContainsKey(x.ToString())).Select(x => Codes.Code[x.ToString()]).ToList();
					return GetResult(code128BKeys, 104);

				case CodeTypes.Code128C:
					if (barcode.Any(x => x > '9' || x < '0')) return new List<BarcodeItem>();

					var elementsKeyList = new List<int>();
					var avg = (barcode.Length + 1) / 2;
					Enumerable.Range(0, avg).ToList().ForEach(i =>
					{
						var bc = barcode.Skip(i * 2);
						if (i != avg - 1) bc = bc.Take(2);

						var section = string.Join(string.Empty, bc);
						if (section.Length == 1)
							elementsKeyList.Add(100);

						if (Codes.Code.TryGetValue(section, out var value))
							elementsKeyList.Add(value);
					});

					return GetResult(elementsKeyList, 105);

				case CodeTypes.Code39:
					barcode = $"*{barcode}*";
					var result = new List<BarcodeItem>();

					foreach (var item in barcode)
					{
						if (!Codes.Code39.TryGetValue(item.ToString(), out var codeItem)) return result;
						codeItem += 'n';
						for (var i = 0; i < codeItem.Length; i++)
						{
							result.Add(new BarcodeItem
							{
								Color = (codeItem[i] == 'W' || codeItem[i] == 'N') ? Brushes.Black : Brushes.Transparent,
								Width = ((codeItem[i] == 'n' || codeItem[i] == 'N') ? 1 : Code39WideRate) * BarWidth
							});
						}
					}
					return result;
				default:
					return new List<BarcodeItem>();
			}
		}

		private List<BarcodeItem> GetResult(List<int> elementsKeyList, int crc)
		{
			var sum = (elementsKeyList.Select((x, i) => x * (i + 1)).Sum() + crc) % 103;
			elementsKeyList.Insert(0, crc);
			elementsKeyList.Add(sum);
			elementsKeyList.Add(106);
			return GetBarcodeItems(elementsKeyList);
		}

		private List<BarcodeItem> GetBarcodeItems(List<int> elementsKeyList)
		{
			var result = new List<BarcodeItem>();
			elementsKeyList.ForEach(key => result.AddRange(GetBarcodeItems(Codes.Band[key])));
			return result;
		}

		private List<BarcodeItem> GetBarcodeItems(string bandcode) => bandcode.Select((x, i) => new BarcodeItem
		{
			Color = i % 2 == 0 ? Brushes.Black : Brushes.Transparent,
			Width = int.Parse(bandcode[i].ToString()) * BarWidth
		}).ToList();
	}
}