using System;

namespace PoneCrafter.Json {

[Serializable]
public class BaseJsonModel {
    public string uuid;
    public string type;
    
    public Guid GetUuid() {
        return new Guid(uuid);
    }
}

}