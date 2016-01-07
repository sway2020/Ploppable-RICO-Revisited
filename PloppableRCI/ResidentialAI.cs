using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using ColossalFramework.Plugins;
using System;
using UnityEngine;
using ICities;

namespace PloppableRICO
{

	public class PloppableResidential : ResidentialBuildingAI
	{
		public int m_levelmin = 1;
		public int m_levelmax = 1;

		public int m_housemulti = 0;

		//PloppableRICO.BuildingData bdata;
	
		public int BID = 2;

		public int timer = 0;

		string OriginalName;


		public override void GetWidthRange (out int minWidth, out int maxWidth)
		{
			base.GetWidthRange (out minWidth, out maxWidth);
			minWidth = 1;
			maxWidth = 32;
		}

		public override void GetDecorationArea (out int width, out int length, out float offset)
		{
			base.GetDecorationArea (out width, out length, out offset);
			width = this.m_info.m_cellWidth;
			length = this.m_info.m_cellLength;
			offset = 0f;
		}

		public override void GetLengthRange (out int minLength, out int maxLength)
		{
			base.GetLengthRange (out minLength, out maxLength);
			minLength = 1;
			maxLength = 16;
		}

		public override int CalculateHomeCount (Randomizer r, int width, int length)
		{
			return base.CalculateHomeCount (r, width + this.m_housemulti, length + this.m_housemulti);
		}

		public override void SimulationStep (ushort buildingID, ref Building data)
		{
			data.UpdateBuilding ((ushort)data.m_buildIndex);
		
			if (timer == 0) {
				OriginalName = data.Info.name; 
				timer = 1;
			}

			if (data.Info.m_class.m_service != ItemClass.Service.Residential) {

				if ((ushort)m_levelmin >= data.m_customBuffer2) {
					data.m_customBuffer2 = (ushort)m_levelmin; // Set the minimum level
				}

				if (data.m_customBuffer2 == 0) {
					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level1");
					Debug.Log (data.Info.name + "Level1 assined");
				}

				if (data.m_customBuffer2 == 2) {
					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level2");
					;
				}
				if (data.m_customBuffer2 == 3) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level3");

				}
				if (data.m_customBuffer2 == 4) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level4");

				}
				if (data.m_customBuffer2 == 5) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level5");
				}

			}
				
		
			data.m_flags &= ~Building.Flags.ZonesUpdated;
			data.m_problems = Notification.Problem.None;
			data.m_flags &= ~Building.Flags.Abandoned;

			data.m_garbageBuffer = 0;
			data.m_fireHazard = 0;
			data.m_fireIntensity = 0;

			if ((ushort)m_levelmax == data.m_customBuffer2) { ///If Its reached max level, then dont level up. 
				data.m_levelUpProgress = 240;
			}
				
			if (data.m_levelUpProgress >= 253) {

				if (data.m_customBuffer2 == 0) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level2");
					data.m_customBuffer2 = 2;

				} else if (data.m_customBuffer2 == 2) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level3");
					data.m_customBuffer2 = 3;

				} else if (data.m_customBuffer2 == 3) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level4");
					data.m_customBuffer2 = 4;

				} else if (data.m_customBuffer2 == 4) {

					data.Info = PrefabCollection<BuildingInfo>.FindLoaded (OriginalName + "_Level5");
					data.m_customBuffer2 = 5;

				}
				data.m_levelUpProgress = 240; //once leveled, set the building back to normal level up progress. 
			}
	

			base.SimulationStep (buildingID, ref data);


		}

	}
}