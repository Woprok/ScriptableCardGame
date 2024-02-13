using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Shared.Common.Languages;
using Shared.Common.Models;
using Shared.Game.Entities;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.ViewModels
{
    public class CardCreatorViewModel : ViewModelBase, ICardCreator
    {
        private static string defaultText = "using Shared.Game.Entities.Cards.Default; " + Environment.NewLine +
                                            "using Shared.Game.Entities.Cards.Interfaces; " + Environment.NewLine +
                                            "using Shared.Game.Entities.Cards.CardClasses; " + Environment.NewLine +
                                            "ICard card = new Card(Account,\"DUMMY BOSS\",true); " + Environment.NewLine +
                                            "card.Class = new MinionCardClass(1,10);" + Environment.NewLine +
                                            "card.SetBrush(Brushes.LightGray);" + Environment.NewLine +
                                            "return card;";

        private string currentText;
        private ObservableCollection<string> compilationErrors = new ObservableCollection<string>();
        private bool canCompile;

        public event EventHandler<ICard> NewCardCreated;

        /// <summary>
        /// It would be cool if this implemented better editor
        /// todo implement more cool editor
        /// </summary>
        public CardCreatorViewModel()
        {
            CurrentText = defaultText;
            CurrentScriptOptions = BuildScriptOptions();
        }

        public Player ThisPlayer { get; set; }
        
        public ScriptOptions BuildScriptOptions()
        {
            ScriptOptions scriptOptions = ScriptOptions.Default; //this is immutable, good to know...
            ScriptOptions scripter = scriptOptions.AddReferences(GetAssembliesForReference());
            scripter = scripter.AddImports("System.Windows.Media");
            return scripter;
        }

        public IEnumerable<Assembly> GetAssembliesForReference()
        {
            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            assemblies.Add(Assembly.GetExecutingAssembly());
            return assemblies;
        }

        public ScriptOptions CurrentScriptOptions { get; private set; }
        public Script<ICard> CurrentScript { get; private set; }

        public void NewCard()
        {
            //this is not securde to be valid card on all fronts
            bool wasCreated;
            ICard newCard = null;
            try
            {
                newCard = CurrentScript.RunAsync(globals: ThisPlayer).Result.ReturnValue;
                wasCreated = true;
            }
            catch (Exception e) //Gotta catch em all, I guess this would be pain to catch one by one and right now it does not matter why.
            {
                wasCreated = false;
            }

            if (wasCreated)
            {
                NewCardCreated?.Invoke(this, newCard);
            }
            else
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(CardCreatorViewModel), "Error occured during execution of card code. New card will not be created."),
                    LanguageHelper.TranslateContextual(nameof(CardCreatorViewModel), "Compilation error!"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            CanCompile = false;
        }

        public void TestCard()
        {
            Task.Run(() =>
            {
                CurrentScript = CSharpScript.Create<ICard>(CurrentText, CurrentScriptOptions, globalsType: typeof(Player));

                ImmutableArray<Diagnostic> diagnostics = CurrentScript.Compile();

                Application.Current.Dispatcher.Invoke(() => CompilationErrors.Clear());

                if (diagnostics.IsEmpty)
                {
                    CanCompile = true;
                }
                else
                {
                    foreach (Diagnostic diagnostic in diagnostics)
                    {
                        Application.Current.Dispatcher.Invoke(() => CompilationErrors.Add(diagnostic.GetMessage()));
                    }
                }
            });
        }

        public void ResetCard()
        {
            CurrentText = defaultText;
        }

        public override string this[string columnName]
        {
            get
            {
                if (columnName == nameof(CurrentText))
                {
                    CanCompile = false;
                }
                return base[columnName];
            }
        }

        public string CurrentText
        {
            get => currentText;
            set
            {
                if (currentText == value) return;
                currentText = value; 
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> CompilationErrors
        {
            get => compilationErrors;
            set
            {
                if (compilationErrors == value) return;
                compilationErrors = value;
                OnPropertyChanged();
            }
        }
        public bool CanCompile
        {
            get => canCompile;
            set
            {
                if (canCompile == value) return;
                canCompile = value;
                OnPropertyChanged();
            }
        }
    }
}