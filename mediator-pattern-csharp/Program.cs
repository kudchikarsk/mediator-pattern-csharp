using System;

class Program
{
    static void Main(string[] args)
    {
        IMediator mediator = new NetworkMediator();
        ComputerColleague colleague1 = new ComputerColleague("Eagle");
        ComputerColleague colleague2 = new ComputerColleague("Ostrich");
        ComputerColleague colleague3 = new ComputerColleague("Penguin");
        mediator.Register(colleague1);
        mediator.Register(colleague2);
        mediator.Register(colleague3);
        mediator.Unregister(colleague1);
        Console.ReadLine();

        /*
        Output:
        New Computer register event with name:Ostrich: received @Eagle
        New Computer register event with name:Penguin: received @Eagle
        New Computer register event with name:Penguin: received @Ostrich
        Computer left unregister event with name:Eagle:received @Ostrich
        Computer left unregister event with name:Eagle:received @Penguin
        */
    }

    /* Define the contract for communication between Colleagues.
    Implementation is left to ConcreteMediator */
    interface IMediator
    {
        void Register(Colleague colleague);
        void Unregister(Colleague colleague);
    }


    /* Act as a central hub for communication between different Colleagues.
     Notifies all Concrete Colleagues on the occurrence of an event
    */
    class NetworkMediator : IMediator
    {
        public event EventHandler<ColleagueArgs> RegisterNotification = delegate { };
        public event EventHandler<ColleagueArgs> UnRegisterNotification = delegate { };
        public NetworkMediator()
        {
        }
        public void Register(Colleague colleague)
        {
            RegisterNotification(this, new ColleagueArgs(colleague));
            RegisterNotification += colleague.ReceiveRegisterNotification;
            UnRegisterNotification += colleague.ReceiveUnRegisterNotification;
        }
        public void Unregister(Colleague colleague)
        {
            RegisterNotification -= colleague.ReceiveRegisterNotification;
            UnRegisterNotification -= colleague.ReceiveUnRegisterNotification;
            UnRegisterNotification(this, new ColleagueArgs(colleague));
        }
    }

    public class ColleagueArgs : EventArgs
    {
        public ColleagueArgs(Colleague colleague)
        {
            Colleague = colleague;
        }

        public Colleague Colleague { get; }
    }

    /* Define the contract for notification events from Mediator.
     Implementation is left to ConcreteColleague
    */
    public abstract class Colleague
    {
        private String name;
        public Colleague(String name)
        {
            this.name = name;
        }
        public override String ToString()
        {
            return name;
        }
        public abstract void ReceiveRegisterNotification(
            object sender, ColleagueArgs colleagueArgs);
        public abstract void ReceiveUnRegisterNotification(
            object sender, ColleagueArgs colleagueArgs);
    }

    /* Process notification event raised by other Colleague through Mediator.
    */
    class ComputerColleague : Colleague
    {
        public ComputerColleague(string name) : base(name)
        {
        }

        public override void ReceiveRegisterNotification(
            object sender, ColleagueArgs colleagueArgs)
        {
            Console.WriteLine("New Computer register event with name:"
                + colleagueArgs.Colleague + ": received @" + this);
            // Send further messages to this new Colleague from now onwards
        }

        public override void ReceiveUnRegisterNotification(
            object sender, ColleagueArgs colleagueArgs)
        {
            Console.WriteLine("Computer left unregister event with name:"
                + colleagueArgs.Colleague + ":received @" + this);
            // Do not send further messages to this Colleague from now onwards
        }
    }
}