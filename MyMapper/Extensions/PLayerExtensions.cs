using MyMapper.Models;
using MyMapper.Entities;
namespace MyMapper.Extensions
{
	public static class PLayerMapperExtensions
	{
		public static PLayer ToPLayer(this PLayerDTO pLayerDTO)
		{
			PLayer pLayer = new PLayer(); 
			pLayer.MyProperty1 = pLayerDTO.MyProperty1;
			pLayer.MyProperty2 = pLayerDTO.MyProperty2;
			return pLayer;
		}
		public static PLayerDTO ToPLayerDTO(this PLayer pLayer)
		{
			PLayerDTO pLayerDTO = new PLayerDTO(); 
			pLayerDTO.MyProperty1 = pLayer.MyProperty1;
			pLayerDTO.MyProperty2 = pLayer.MyProperty2;
			return pLayerDTO;
		}
	}
}

