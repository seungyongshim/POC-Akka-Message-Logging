using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {
  loglevel = ""DEBUG""
  stdout-loglevel = ""DEBUG""
  actor {
    debug {
      receive = true
    }
  }
}");
            var actorSystem = ActorSystem.Create("root", config);

            var pongActor = actorSystem.ActorOf(Props.Create<PongActor>(), nameof(PongActor));
            var pingActor = actorSystem.ActorOf(Props.Create(() => new PingActor(pongActor)), nameof(PingActor));

            pingActor.Tell(new Person());

            Console.ReadLine();
        }
    }

    public class Person
    {
        public string Id { get; }
        public string Password { get; }
    }

    public class PingActor : ReceiveActor, ILogReceive
    {
        ILoggingAdapter Logger { get; } = Context.GetLogger();

        public PingActor(IActorRef pongActor)
        {
            Receive<Person>(x => pongActor.Tell(x));
        }
    }


    public class PongActor : ReceiveActor, ILogReceive
    {
        ILoggingAdapter Logger { get; } = Context.GetLogger();

        public PongActor()
        {
        }
    }
}
