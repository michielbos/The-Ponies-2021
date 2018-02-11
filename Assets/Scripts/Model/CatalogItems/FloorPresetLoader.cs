
/// <summary>
/// Class for loading the floor presets from the resources, as well as the mod packages.
/// </summary>
public class FloorPresetLoader : PresetLoader<FloorPreset, FloorPresetData> {
	const string floorsFolder = "Floors/";
	const string floorExtension = ".floor";

	public FloorPresetLoader () : 
		base(floorsFolder, floorExtension) { }

	public override FloorPreset CreateFromData (FloorPresetData presetData) {
		return new FloorPreset(presetData);
	}

	protected override bool ValidatePreset (FloorPreset preset) {
		//We are probably missing some checks...
		return true;
	}

}
