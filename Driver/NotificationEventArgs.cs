using System;

namespace Driver
{
    public class NotificationEventArgs : EventArgs
    {
        public string ActionName { get; }
        public int CurrentStep { get; }
        public int NumberOfSteps { get; }
        public ProgramService.EventType ActionType { get; }

        public NotificationEventArgs(string actionName, int currentStep, int numberOfSteps, ProgramService.EventType actionType)
        {
            ActionName = actionName;
            ActionType = actionType;
            CurrentStep = currentStep;
            NumberOfSteps = numberOfSteps;
            ActionType = actionType;
        }
    }
}
