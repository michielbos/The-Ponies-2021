using System;

namespace PoneCrafter.Model {

public class BaseModel {
    public Guid uuid;

    public BaseModel(Guid uuid) {
        this.uuid = uuid;
    }
}

}