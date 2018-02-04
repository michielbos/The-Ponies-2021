using UnityEngine;

/// <summary>
/// The "dummy" of a wall, used for displaying and interaction.
/// It should only be controlled by its owner, which is in the wall attribute.
/// </summary>
public class WallDummy : MonoBehaviour {
    public Wall wall;

    //Not sure what this does.
    //Move it to the right place or remove it.
    /*public WallDummy (TerrainTile s, TerrainTile e) {
        /*start and end should be interchangable
          assume that gven tiles are right
          direction 0:・,1:-,2:\,3:|,4:/
         */ /*
        startTile = s;
        endTile = e;
        int xdif = (e.x - s.x);
        int ydif = Mathf.Abs( e.y - s.y);
        if (ydif != 0)
        {
            direction = 3;
            if (xdif != 0) direction += (int)Mathf.Sign(xdif);
        }
        else if (xdif != 0) direction = 1;
        if (xdif == 0 || ydif == 0)
        {
            length =Mathf.Abs( xdif + ydif);
        }else length=ydif;
        x = Mathf.Min(s.x, e.x);
        y = Mathf.Min(s.y, e.y); 
    }*/

}
