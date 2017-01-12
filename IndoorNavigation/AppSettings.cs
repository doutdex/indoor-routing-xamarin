﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IndoorNavigation
{
	public class AppSettings
	{
		/// <summary>
		/// Gets or sets the item identifier.
		/// </summary>
		/// <value>Portal Item ID</value>
		[XmlElement]
		public string ItemID
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the name of the item.
		/// </summary>
		/// <value>The name of the Portal item</value>
		[XmlElement]
		public string ItemName
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the mmpk date.
		/// </summary>
		/// <value>The date the mobile map package was downloaded</value>
		[XmlElement]
		public DateTime MmpkDate
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the home location.
		/// </summary>
		/// <value>The home location set by the user. By default this is set to "Set home location"</value>
		[XmlElement]
		public string HomeLocation
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:IndoorNavigation.AppSettings"/> is location services enabled.
		/// </summary>
		/// <value><c>true</c> if is location services switch enabled; otherwise, <c>false</c>.</value>
		[XmlElement]
		public bool IsLocationServicesEnabled
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:IndoorNavigation.AppSettings"/> is prefer elevators enabled.
		/// </summary>
		/// <value><c>true</c> if is prefer elevators switch is enabled; otherwise, <c>false</c>.</value>
		[XmlElement]
		public bool IsPreferElevatorsEnabled
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the index of the rooms layer.
		/// </summary>
		/// <value>The index of the rooms layer.</value>
		[XmlElement]
		public int RoomsLayerIndex
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the index of the floorplan lines layer.
		/// </summary>
		/// <value>The index of the floorplan lines layer.</value>
		[XmlElement]
		public int FloorplanLinesLayerIndex
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the zoom level to display room layers.
		/// </summary>
		/// <value>The zoom level to display room layers.</value>
		[XmlElement]
		public float ZoomLevelToDisplayRoomLayers
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the name of the floor column in rooms tabel.
		/// </summary>
		/// <value>The floor column in rooms tabel.</value>
		public string FloorColumnInRoomsTable
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the home coordinates.
		/// </summary>
		/// <value>The coordinates and floor level for the home location. This also includes the WKID</value>
		[XmlArray("HomeCoordinates")]
		public CoordinatesKeyValuePair<string, double>[] HomeCoordinates
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the initial viewpoint coordinates.
		/// </summary>
		/// <value>The initial viewpoint coordinates used for the map.</value>
		[XmlArray("InitialViewpointCoordinates")]
		public CoordinatesKeyValuePair<string, double>[] InitialViewpointCoordinates
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the locator fields. If there is only one locator, make a list with one value 
		/// </summary>
		/// <value>The locator fields.</value>
		[XmlArray("LocatorFields")]
		public List<string> LocatorFields
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the contact card display fields. These are what is displayed on the Contact card when user searches or taps an office
		/// </summary>
		/// <value>The contact card display fields.</value>
		[XmlArray("ContactCardDisplayFields")]
		public List<string> ContactCardDisplayFields
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the minimum scale of the map.
		/// </summary>
		/// <value>The minimum scale.</value>
		[XmlElement]
		public int MinScale
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the maximum scale of the map.
		/// </summary>
		/// <value>The max scale.</value>
		[XmlElement]
		public int MaxScale
		{
			get; set;
		}


		/// <summary>
		/// Gets or sets the current settings.
		/// </summary>
		/// <value>Static instance of the settings for the application</value>
		public static AppSettings CurrentSettings { get; set; }

		/// <summary>
		/// Loads the app settings if the file exists, otherwise it creates default settings. 
		/// </summary>
		/// <returns>The app settings.</returns>
		/// <param name="filePath">File path.</param>
		internal static async Task<AppSettings> CreateAsync(string filePath)
		{
			// Get all the files in the device directory
			List<string> files = Directory.EnumerateFiles(Path.GetDirectoryName(filePath)).ToList();

			// If the settings file doesn't exist, create it
			if (!files.Contains(filePath))
			{
				var appSettings = new AppSettings();
				appSettings.ItemID = "018f779883434a8daadfb51524ec3498";
				appSettings.ItemName = "EsriCampus.mmpk";
				appSettings.MmpkDate = new DateTime(1900, 1, 1);
				appSettings.HomeLocation = "Set home location";
				appSettings.IsLocationServicesEnabled = false;
				appSettings.IsPreferElevatorsEnabled = false;
				appSettings.RoomsLayerIndex = 1;
				appSettings.FloorplanLinesLayerIndex = 2;
				appSettings.ZoomLevelToDisplayRoomLayers = 500;
				appSettings.FloorColumnInRoomsTable = "FLOOR";
				appSettings.MinScale = 100;
				appSettings.MaxScale = 13000;
				CoordinatesKeyValuePair<string, double>[] initialViewpointCoordinates =
				{
				new CoordinatesKeyValuePair<string, double>("X", -13046209),
				new CoordinatesKeyValuePair<string, double>("Y", 4036456),
				new CoordinatesKeyValuePair<string, double>("WKID", 3857),
				new CoordinatesKeyValuePair<string, double>("ZoomLevel", 1600),
				};
				appSettings.InitialViewpointCoordinates = initialViewpointCoordinates;

				appSettings.LocatorFields = new List<string>() { "LONGNAME", "KNOWN_AS_N" };

				// Information in these fields gets displayed in the contact card
				// List the Item you want searcheable and in bold to be first
				appSettings.ContactCardDisplayFields = new List<string>() { "LONGNAME", "KNOWN_AS_N" };

				var serializer = new XmlSerializer(appSettings.GetType());

				// Create settings file on a separate thread
				// this does not need to be awaited since the return is already set
			    await Task.Factory.StartNew(delegate
				{
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						serializer.Serialize(fileStream, appSettings);
					}
				});
				return appSettings;
			}
			// Otherwise load the settings from the settings file
			else
			{
				using (var fileStream = new FileStream(filePath, FileMode.Open))
				{
					var appSettings = new AppSettings();
					var serializer = new XmlSerializer(appSettings.GetType());
					return serializer.Deserialize(fileStream) as AppSettings;
				}
			}
		}

		/// <summary>
		/// Saves the settings.
		/// </summary>
		/// <param name="filePath">File path.</param>
		internal static void SaveSettings(string filePath)
		{
			var serializer = new XmlSerializer(CurrentSettings.GetType());

			using (var fileStream = new FileStream(filePath, FileMode.Open))
			{
				serializer.Serialize(fileStream, CurrentSettings);
			}
		}

	}


}