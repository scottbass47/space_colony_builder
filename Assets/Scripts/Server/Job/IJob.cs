using System.Collections;
using System.Collections.Generic;
using ECS;

namespace Server.Job
{
    public interface IJob 
    {
        // Called before all other methods
        void SetEntity(Entity entity);

        // Called once after SetEntity but before Update
        void Init();

        void OnUpdate(float delta);
        bool IsFinished();
    }
}