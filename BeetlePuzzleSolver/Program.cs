using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeetlePuzzleSolver
{
    internal enum Colour
    {
        PURPLE, GREEN, ORANGE, YELLOW, BLUE
    }

    internal class Beetle
    {
        public Colour head { get; set; }
        public Colour neck { get; set; }
        public Colour body { get; set; }

        public Beetle(Colour head, Colour neck, Colour body)
        {
            this.head = head;
            this.neck = neck;
            this.body = body;
        }

        public Beetle clone()
        {
            return new Beetle(head, neck, body);
        }

        public bool isMatch()
        {
            return head == neck && neck == body;
        }

        protected bool Equals(Beetle other)
        {
            return head == other.head && neck == other.neck && body == other.body;
        }

        public override bool Equals(object obj)
        {
            //Automatically generated with ReSharper
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Beetle)obj);
        }

        public override int GetHashCode()
        {
            //Automatically generated with ReSharper
            unchecked
            {
                var hashCode = (int)head;
                hashCode = (hashCode * 397) ^ (int)neck;
                hashCode = (hashCode * 397) ^ (int)body;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.head.ToString() + "," + this.neck.ToString() + "," + this.body.ToString();
        }
    }

    internal class Game
    {
        public Beetle swapspace { get; set; }
        public Beetle top { get; set; }
        public Beetle right { get; set; }
        public Beetle down { get; set; }
        public Beetle left { get; set; }

        public int sollength { get; set; }
        public List<String> history { get; set; } = new List<String>();

        public Game(Beetle swapspace, Beetle top, Beetle left, Beetle right, Beetle down)
        {
            this.swapspace = swapspace;
            this.top = top;
            this.right = right;
            this.down = down;
            this.left = left;
            this.sollength = 0;
        }

        public bool isValid()
        {
            Beetle[] games = new[] { this.swapspace, this.top, this.left, this.right, this.down };
            for (int i = 0; i < 5; i++)
                for (int j = i + 1; j < 5; j++)
                {
                    var first = games[i];
                    var second = games[j];
                    if (first.head == second.head || first.neck == second.neck || first.body == second.body)
                        return false;
                }
            return true;
        }

        public Game clone()
        {
            Game copy = new Game(swapspace.clone(), top.clone(), left.clone(), right.clone(), down.clone());
            copy.sollength = this.sollength;
            copy.history = new List<string>(history);
            return copy;
        }

        public bool IsWon()
        {
            return swapspace.isMatch() && left.isMatch() && right.isMatch() && top.isMatch() && down.isMatch();
        }

        public void swap()
        {
            Beetle temp = swapspace;
            swapspace = top;
            top = temp;
            this.sollength++;
            history.Add("swap");
        }

        public void rothead()
        {
            Colour temp = top.head;
            top.head = left.head;
            left.head = down.head;
            down.head = right.head;
            right.head = temp;
            this.sollength++;
            history.Add("head");
        }

        public void rotneck()
        {
            Colour temp = top.neck;
            top.neck = left.neck;
            left.neck = down.neck;
            down.neck = right.neck;
            right.neck = temp;
            this.sollength++;
            history.Add("neck");
        }

        public void rotbody()
        {
            Colour temp = top.body;
            top.body = left.body;
            left.body = down.body;
            down.body = right.body;
            right.body = temp;
            this.sollength++;
            history.Add("body");
        }

        protected bool Equals(Game other)
        {
            return Equals(swapspace, other.swapspace) && Equals(top, other.top) && Equals(right, other.right) && Equals(down, other.down) && Equals(left, other.left);
        }

        public override bool Equals(object obj)
        {
            //Automatically generated with ReSharper
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Game)obj);
        }

        public override int GetHashCode()
        {
            //Automatically generated with ReSharper
            unchecked
            {
                var hashCode = (swapspace != null ? swapspace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (top != null ? top.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (right != null ? right.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (down != null ? down.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (left != null ? left.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.swapspace.ToString() + ", " + this.top.ToString() + ", " + this.left.ToString() + ", " +
                   this.right.ToString() + ", " + this.down.ToString();
        }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            Game startposition = new Game(//TODO place your beetle config here in this order:
                                          //swapspace, top, left, right, down
                                          //beetle colours in this order:
                                          //head, neck, body
                            new Beetle(Colour.ORANGE, Colour.ORANGE, Colour.GREEN),
                            new Beetle(Colour.PURPLE, Colour.PURPLE, Colour.ORANGE),
                            new Beetle(Colour.YELLOW, Colour.YELLOW, Colour.YELLOW),
                            new Beetle(Colour.GREEN, Colour.GREEN, Colour.PURPLE),
                            new Beetle(Colour.BLUE, Colour.BLUE, Colour.BLUE)
                            );
            bool solvepuzzle = true; //Set this to false to generate some general stats

            if (!startposition.isValid())
            {
                Console.WriteLine("ERROR: the starting position is invalid");
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("press any key to exit");
                    Console.ReadKey();
                }
                return;
            }

            Queue<Game> queue = new Queue<Game>();
            queue.Enqueue(startposition);
            HashSet<Game> knowngames = new HashSet<Game>(); //Record every known game position 
            knowngames.Add(startposition);
            while (queue.Count != 0)
            {
                Game current = queue.Dequeue();
                Game[] copies = new Game[4];
                for (int i = 0; i < 4; i++)
                    copies[i] = current.clone();

                copies[0].swap();
                copies[1].rothead();
                copies[2].rotneck();
                copies[3].rotbody();

                foreach (var copy in copies)
                    if (solvepuzzle && copy.IsWon()) //If we do not check for winning, then we generate every valid position for some stats
                    {
                        Console.WriteLine(copy.history.Count + " steps:");
                        for (int i = 0; i < copy.history.Count; i++)
                            Console.WriteLine((i + 1) + ". : " + copy.history[i]);
                        Console.WriteLine("Checked " + knowngames.Count + " positions");
                        if (Debugger.IsAttached)
                        {
                            Console.WriteLine("press any key to exit");
                            Console.ReadKey();
                        }
                        return;
                    }
                    else if (!knowngames.Contains(copy))
                    {
                        queue.Enqueue(copy); //Only add unknown game positions
                        knowngames.Add(copy);
                    }

            }

            //
            // All of the following is merely for generating general stats
            // That will cause the generation of every single possible game state
            // Note that (5!)^3 == 120^3 == 1 728 000 == number of positions below
            // So we do indeed generate every possible position
            //

            Console.WriteLine("Number of positions: " + knowngames.Count);
            var solutions = knowngames.AsParallel().Where(x => x.IsWon());
            Console.WriteLine("Number of solving positions: " + solutions.Count());
            Console.WriteLine("Max distance in graph: " + knowngames.AsParallel().Select(x => x.sollength).Max());

            HashSet<Game> newset = new HashSet<Game>();

            foreach (var solution in solutions)
            {
                solution.sollength = 0;
                queue.Enqueue(solution);
                newset.Add(solution);
            }

            while (queue.Count != 0)
            {
                Game current = queue.Dequeue();
                Game[] copies = new Game[4];
                for (int i = 0; i < 4; i++)
                    copies[i] = current.clone();

                copies[0].swap();
                copies[1].rothead();
                copies[1].rothead();
                copies[1].rothead();
                copies[1].sollength -= 2; //This is rotating backwards
                copies[2].rotneck();
                copies[2].rotneck();
                copies[2].rotneck();
                copies[2].sollength -= 2;
                copies[3].rotbody();
                copies[3].rotbody();
                copies[3].rotbody();
                copies[3].sollength -= 2;

                foreach (var copy in copies)
                {
                    if (!newset.Contains(copy))
                    {
                        queue.Enqueue(copy); 
                        newset.Add(copy);
                    }
                }
            }
            Console.WriteLine("A case with maximum solution length:");
            Console.WriteLine(newset.AsParallel().First(x => x.sollength == 16)); //One case with maximum solution length
            Console.WriteLine("Avg distance to solution: " + newset.AsParallel().Select(x => x.sollength).Average());
            Console.WriteLine("Max distance to solution: " + newset.AsParallel().Select(x => x.sollength).Max());

            if (Debugger.IsAttached)
            {
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
