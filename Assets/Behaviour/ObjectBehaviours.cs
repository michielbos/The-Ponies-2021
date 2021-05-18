using System;
using System.Collections.Generic;
using Model.Behaviours;
using Model.Property;
using ThePoniesBehaviour.Behaviours;

namespace ThePoniesBehaviour {

public class ObjectBehaviours : IObjectBehaviourProvider {
    public IEnumerable<Type> GetBehaviours(PropertyObject target) {
        if (target.Type == "stereo") {
            return new[] {typeof(StereoBehaviour)};
        }
        return null;
    }
}

}