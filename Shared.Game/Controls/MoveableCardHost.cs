using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Shared.Common.Languages;
using Shared.Game.Helpers;
using Shared.Game.Views;

namespace Shared.Game.Controls
{
    /// <summary>
    /// ContentControl that can be moved inside of his parent, extremly dependant on cardHolder
    /// </summary>
    public class MoveableCardHost : ContentControl
    {
        protected Point Start;
        protected Point Relative;
        protected Point DistanceFromStart;
        protected bool FirstClick = true;
        protected Point BackUpCoordinates = new Point();
        protected Point BackUpTranslation = new Point();

        public event Action<MoveableCardHost> OnMovementEnd;

        /// <summary>
        /// Reference for true parent. This parent holds reference while this element exists and directly own this element if is this element moving.
        /// </summary>
        public GameBoardGrid PernamentParent { get; set; }
        /// <summary>
        /// Reference for parent that hold him.
        /// </summary>
        public CardHolderBorder LastCardHolder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICardView DisplayedCard { get; set; }
        public bool IsMoving { get; set; }

        public MoveableCardHost()
        {
            RenderTransform = new TranslateTransform();
            MouseDown += OnMovementStart;
            MouseMove += OnMove;
            MouseUp += OnMovementStop;
        }
        
        /// <summary>
        /// Remove host from existence
        /// </summary>
        public void DestroyMoveableHost()
        {
            MouseDown -= OnMovementStart;
            MouseMove -= OnMove;
            MouseUp -= OnMovementStop;
            PernamentParent.RemoveMoveableHost(this);

            //Remove self from parent
            PernamentParent.Children.Remove(this);

            //Remove any remaining reference
            LastCardHolder.Child = null;
            LastCardHolder = null;
        }

        /// <summary>
        /// Return to this.LastCardHolder
        /// </summary>
        public void ReturnToLastHolder()
        {
            TranslateTransform MoveTransform = this.RenderTransform as TranslateTransform;

            Point pointForCentering = LastCardHolder.TranslatePoint(new Point(0, 0), this);

            double sizeDifferenceWidth = Math.Abs(this.Width - LastCardHolder.Width) / 2;
            double sizeDifferenceHeight = Math.Abs(this.Height - LastCardHolder.Height) / 2;

            MoveTransform.X = MoveTransform.X + pointForCentering.X + sizeDifferenceWidth;
            MoveTransform.Y = MoveTransform.Y + pointForCentering.Y + sizeDifferenceHeight;

            GeneralTransform transform = PernamentParent.TransformToVisual(LastCardHolder as Visual);
            Point translation = transform.Transform(new Point(0, 0));

            PernamentParent.Children.Remove(this);
            LastCardHolder.Child = this;
            this.LastCardHolder = LastCardHolder;
            this.VisualizeOnTemporaryParent(translation);
        }

        /// <summary>
        /// Straigth initialization of Content property
        /// </summary>
        /// <param name="content"></param>
        public MoveableCardHost(ICardView card) : this()
        {
            DisplayedCard = card;
            Content = card;
        }

        public void OnMouseLeftWindow(object sender, MouseEventArgs e)
        {
            if (!IsMoving)
                return;
            IsMoving = false;
            ReturnToLastHolder();
        }

        public void SetToPlace(GameBoardGrid gameHolder, CardHolderBorder cardHolder)
        {
            PernamentParent = gameHolder;

            LastCardHolder = cardHolder;
            LastCardHolder.Child = this;
        }

        public bool CanExecuteMovement()
        {
            StringBuilder messageBuilder = new StringBuilder();
            if (PernamentParent.ThisPlayer.Account != DisplayedCard.ContextModel.ContentCard.CurrentOwner)
            {
                messageBuilder.AppendLine(LanguageHelper.TranslateContextual(nameof(MoveableCardHost), "You are not owner."));
            }
            if (!DisplayedCard.ContextModel.ContentCard.CanBeMoved)
            {
                messageBuilder.AppendLine(LanguageHelper.TranslateContextual(nameof(MoveableCardHost), "Card can't be moved."));
            }

            if (!PernamentParent.CanPlay)
            {
                messageBuilder.AppendLine(LanguageHelper.TranslateContextual(nameof(MoveableCardHost), "Can't play card during other players turns."));
            }
            if (string.IsNullOrEmpty(messageBuilder.ToString()))
            {
                return true;
            }

            string errorString = LanguageHelper.TranslateContextual(nameof(MoveableCardHost), "Can't be moved due to:") + Environment.NewLine;

            MessageBox.Show(
                LanguageHelper.TranslateContextual(nameof(MoveableCardHost), errorString + messageBuilder.ToString()),
                LanguageHelper.TranslateContextual(nameof(MoveableCardHost), "Moving Denied!"),
                MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return false;
        }

        private void OnMovementStart(object sender, MouseButtonEventArgs e)
        {
            if (!CanExecuteMovement() || IsMoving)
                return;
            IsMoving = true;

            Point RelativeMousePoint = Mouse.GetPosition(this);
            Relative.X = RelativeMousePoint.X;
            Relative.Y = RelativeMousePoint.Y;

            if (FirstClick) //do magic for first time movement attempt
            {
                FirstClick = false;

                LastCardHolder.Child = null;
                PernamentParent.Children.Add(this);
                PernamentParent.UpdateLayout();

                TranslateTransform MoveTransform = this.RenderTransform as TranslateTransform;
                double sizeDifferenceWidth = Math.Abs(this.Width - LastCardHolder.Width) / 2;
                double sizeDifferenceHeight = Math.Abs(this.Height - LastCardHolder.Height) / 2;

                GeneralTransform aftertransform = TransformToAncestor(PernamentParent as Visual);
                Start = aftertransform.Transform(new Point(0, 0));

                Point coordinates = LastCardHolder.TranslatePoint(new Point(0, 0), this);
                
                Point pointForCentering = coordinates;

                MoveTransform.X = MoveTransform.X + pointForCentering.X + sizeDifferenceWidth;
                MoveTransform.Y = MoveTransform.Y + pointForCentering.Y + sizeDifferenceHeight;

                return;
            }

            if (LastCardHolder != null && Equals(LastCardHolder.Child, this))
            {
                LastCardHolder.Child = null;
                PernamentParent.Children.Add(this);
                VisualizeOnParent();
            }
        }

        public void VisualizeOnParent()
        {
            TranslateTransform MoveTransform = this.RenderTransform as TranslateTransform;
            double sizeDifferenceWidth = Math.Abs(this.Width - LastCardHolder.Width) / 2;
            double sizeDifferenceHeight = Math.Abs(this.Height - LastCardHolder.Height) / 2;

            //Calculate diff
            GeneralTransform transform = PernamentParent.TransformToVisual(LastCardHolder as Visual);
            Point resultPoint = transform.Transform(new Point(MoveTransform.X + sizeDifferenceWidth, MoveTransform.Y + sizeDifferenceHeight));
            //Calculate correct position
            var backPoint = new Point(MoveTransform.X + BackUpTranslation.X + sizeDifferenceWidth, MoveTransform.Y + BackUpTranslation.Y + sizeDifferenceHeight); 

            Point coordinates = BackUpCoordinates;

            if (resultPoint != backPoint) //fix coordninates if scrollmoved
            {
                Point diffPoint = DistanceHelper.GetUnitDistance(resultPoint, backPoint);
                coordinates = new Point(coordinates.X - diffPoint.X, coordinates.Y - diffPoint.Y);
            }

            Point pointForCentering = PernamentParent.TranslatePoint(coordinates, this);

            MoveTransform.X = MoveTransform.X + pointForCentering.X + sizeDifferenceWidth;
            MoveTransform.Y = MoveTransform.Y + pointForCentering.Y + sizeDifferenceHeight;
        }

        public void VisualizeOnTemporaryParent(Point backUpTranslation)
        {
            TranslateTransform MoveTransform = this.RenderTransform as TranslateTransform;
            BackUpCoordinates = new Point(MoveTransform.X, MoveTransform.Y);
            MoveTransform.X = 0;
            MoveTransform.Y = 0;

            BackUpTranslation = backUpTranslation;
        }

        private void OnMove(object sender, MouseEventArgs e)
        {
            if (IsMoving)
            {
                //Get the position of the mouse relative to the controls parent      
                FrameworkElement parentObject = PernamentParent as FrameworkElement;
                Point MousePoint = Mouse.GetPosition(parentObject);

                //set the distance from the original position
                DistanceFromStart.X = MousePoint.X - Start.X - Relative.X;
                DistanceFromStart.Y = MousePoint.Y - Start.Y - Relative.Y;

                //Set the X and Y coordinates of the RenderTransform to be the Distance from original position. This will move the control
                TranslateTransform MoveTransform = RenderTransform as TranslateTransform;
                MoveTransform.X = DistanceFromStart.X;
                MoveTransform.Y = DistanceFromStart.Y;
            }
        }

        private void OnMovementStop(object sender, MouseButtonEventArgs e)
        {
            if (!IsMoving)
                return;
            IsMoving = false;
            OnMovementEnd?.Invoke(this);
        }

        public CardHolderBorder FindClosestHolder(out Point closestHolderToThis)
        {
            closestHolderToThis = new Point(0, 0);
            CardHolderBorder closestHolder = null; //will be initialized in cyle, if not then its legit error

            double minDistance = Double.MaxValue;
            Point relativeCardPoint = this.TransformToAncestor(PernamentParent).Transform(new Point(0, 0));
            //add cardSize
            relativeCardPoint = new Point(relativeCardPoint.X + this.Width / 2, relativeCardPoint.Y + this.Height / 2);

            foreach (CardHolderBorder element in PernamentParent.CardHolders)
            {
                Point relativeBorderPoint = element.TransformToAncestor(PernamentParent).Transform(new Point(0, 0));
                //add borderSize
                relativeBorderPoint = new Point(
                    relativeBorderPoint.X + element.Width / 2,
                    relativeBorderPoint.Y + element.Height / 2);

                double thisElementDistance = DistanceHelper.GetDistance(relativeCardPoint, relativeBorderPoint);

                double triangledDistance = DistanceHelper.GetDistance(element.Width / 2, element.Height / 2);

                if (thisElementDistance < minDistance &&
                    (thisElementDistance < element.Width / 2 || thisElementDistance < element.Height / 2
                                                             || thisElementDistance < triangledDistance))
                {
                    minDistance = thisElementDistance;
                    closestHolderToThis = element.TranslatePoint(new Point(0, 0), this);
                    closestHolder = element;
                }
            }

            return closestHolder;
        }

        public void MoveToHolder(CardHolderBorder foundClosestHolder, Point centeringPoint)
        {
            double sizeDifferenceWidth = Math.Abs(this.Width - foundClosestHolder.Width) / 2;
            double sizeDifferenceHeight = Math.Abs(this.Height - foundClosestHolder.Height) / 2;

            TranslateTransform MoveTransform = this.RenderTransform as TranslateTransform;
            MoveTransform.X = MoveTransform.X + centeringPoint.X + sizeDifferenceWidth;
            MoveTransform.Y = MoveTransform.Y + centeringPoint.Y + sizeDifferenceHeight;

            GeneralTransform transform = PernamentParent.TransformToVisual(foundClosestHolder as Visual);
            Point translation = transform.Transform(new Point(0, 0));

            PernamentParent.Children.Remove(this);
            foundClosestHolder.Child = this;
            this.LastCardHolder = foundClosestHolder;
            this.VisualizeOnTemporaryParent(translation);
        }

        public void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FirstClick = true;
        }
    }
}