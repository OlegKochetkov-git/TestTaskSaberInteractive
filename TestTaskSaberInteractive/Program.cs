class ListNode
{
    public ListNode Previous;
    public ListNode Next;
    public ListNode Random;
    public string Data;
}

class ListRandom
{
    public ListNode Head;
    public ListNode Tail;
    public int Count;

    private ListNode GetNodeAt(int index)
    {
        int counter = 0;
        for (ListNode currentNode = Head; currentNode.Next != null; currentNode = currentNode.Next)
        {
            if (counter == index)
                return currentNode;
            counter++;
        }
        return new ListNode();
    }


    public void Serialize(FileStream s)
    {
        Dictionary<ListNode, int> dictionary = new Dictionary<ListNode, int>();
        int id = 0;
        for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
        {
            dictionary.Add(currentNode, id);
            id++;
        }
        using (BinaryWriter writer = new BinaryWriter(s))
        {
            for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
            {
                writer.Write(currentNode.Data);
                writer.Write(dictionary[currentNode.Random]); // ключ будет ссылка на рандомный элемент
            }
        }
        Console.WriteLine("List serialized");
    }

    public void Deserialize(FileStream s)
    {
        Dictionary<int, Tuple<string, int>> dictionary = new Dictionary<int, Tuple<string, int>>();
        int counter = 0;
        using (BinaryReader reader = new BinaryReader(s))
        {
            while (reader.PeekChar() != -1)
            {
                string data = reader.ReadString();
                int randomId = reader.ReadInt32();
                dictionary.Add(counter, new Tuple<string, int>(data, randomId));
                counter++;
            }
            Console.WriteLine("File readed");
        }
        Count = counter;
        Head = new ListNode();
        ListNode current = Head;
        for (int i = 0; i < Count; i++)
        {
            current.Data = dictionary.ElementAt(i).Value.Item1;
            current.Next = new ListNode();
            if (i != Count - 1)
            {
                current.Next.Previous = current;
                current = current.Next;
            }
            else
            {
                Tail = current;
            }
        }
        counter = 0;
        for (ListNode currentNode = Head; currentNode.Next != null; currentNode = currentNode.Next)
        {
            currentNode.Random = GetNodeAt(dictionary.ElementAt(counter).Value.Item2);
            counter++;
        }
        Console.WriteLine("List deserialized");
    }
}


class Program
{

    static Random rand = new Random();

    //help to create next node
    static ListNode AddNode(ListNode prev)
    {
        ListNode result = new ListNode();
        result.Previous = prev;
        result.Next = null;
        result.Data = rand.Next(0, 100).ToString();
        prev.Next = result;
        return result;
    }

    //help to create ref to Random node
    static ListNode RandomNode(ListNode _head, int _length)
    {
        int k = rand.Next(0, _length);
        int i = 0;
        ListNode result = _head;
        while (i < k)
        {
            result = result.Next;
            i++;
        }
        return result;
    }

    static void Main(string[] args)
    {
        //nodes count for test
        int length = 7;

        //first node
        ListNode head = new ListNode();
        ListNode tail = new ListNode();
        ListNode temp = new ListNode();

        head.Data = rand.Next(0, 1000).ToString();

        tail = head;

        for (int i = 1; i < length; i++)
            tail = AddNode(tail);

        temp = head;

        //add ref to Random node
        for (int i = 0; i < length; i++)
        {
            temp.Random = RandomNode(head, length);
            temp = temp.Next;
        }

        //declare first List
        ListRandom first = new ListRandom();
        first.Head = head;
        first.Tail = tail;
        first.Count = length;

        //serialize it
        FileStream fs = new FileStream("dat.dat", FileMode.OpenOrCreate);
        first.Serialize(fs);

        //deserialize in second List
        ListRandom second = new ListRandom();
        try
        {
            fs = new FileStream("dat.dat", FileMode.Open);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("Press Enter to exit.");
            Console.Read();
            Environment.Exit(0);
        }
        second.Deserialize(fs);

        //if second.Tail`s data equals first.Tail`s data, we guess it`s OK
        if (second.Tail.Data == first.Tail.Data) Console.WriteLine("Success");
        Console.Read();
    }
}