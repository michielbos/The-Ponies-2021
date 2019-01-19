namespace Model.Property {

public class Roof {
	public int x1;
	public int y1;
	public int x2;
	public int y2;
	public int height;
	public int skin;

	public Roof (int x1, int y1, int x2, int y2, int height, int skin) {
		this.x1 = x1;
		this.y1 = y1;
		this.x2 = x2;
		this.y2 = y2;
		this.height = height;
		this.skin = skin;
	}

	public Roof (RoofData roofData) : this(roofData.x1, roofData.y1, roofData.x2, roofData.y2, roofData.height, roofData.skin) {

	}

	public RoofData GetRoofData () {
		return new RoofData(x1,
			y1,
			x2,
			y2,
			height,
			skin);
	}
}

}
