using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class StoryService : Singleton<StoryService> , IDisposable
    {

        public void Init()
        {
            StoryManager.Instance.Init();
        }

        public StoryService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<StoryStartRequest>(this.OnStoryStart);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<StoryEndRequest>(this.OnStoryEnd);
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<StoryStartRequest>(this.OnStoryStart);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<StoryEndRequest>(this.OnStoryEnd);
        }

        private void OnStoryStart(NetConnection<NetSession> sender, StoryStartRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnStoryStart: :storyId:{0}", request.storyId);

            var story = StoryManager.Instance.NewStory(request.storyId, sender);

            sender.Session.Response.storyStart = new StoryStartResponse();
            sender.Session.Response.storyStart.storyId = story.StoryId;
            sender.Session.Response.storyStart.instanceId = story.InstanceId;
            sender.Session.Response.storyStart.Result = Result.Success;
            sender.Session.Response.storyStart.Errormsg = "";
            sender.SendResponse();
        }

        private void OnStoryEnd(NetConnection<NetSession> sender, StoryEndRequest message)
        {
            var story = StoryManager.Instance.GetStory(message.storyId, message.instanceId);
            story.End();
            sender.Session.Response.storyEnd = new StoryEndResponse();
            sender.Session.Response.storyEnd.Result = Result.Success;
            sender.Session.Response.storyEnd.storyId = story.StoryId;
            sender.Session.Response.storyEnd.instanceId = story.InstanceId;
            sender.SendResponse();
        }
    }
}
