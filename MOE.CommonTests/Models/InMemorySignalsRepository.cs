﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOE.Common.Business;
using MOE.Common.Models;
using MOE.Common.Models.Repositories;
using Detector = MOE.Common.Models.Detector;
using Signal = MOE.Common.Models.Signal;

namespace MOE.CommonTests.Models
{
    public class InMemorySignalsRepository : ISignalsRepository
    {
        public DateTime LastDate = Convert.ToDateTime("01/01/9999");

        private InMemoryMOEDatabase _db;

        public InMemorySignalsRepository()
        {
            _db = new InMemoryMOEDatabase();

        }

        public InMemorySignalsRepository(InMemoryMOEDatabase db)
        {
            _db = db;

        }



        public void AddList(List<Common.Models.Signal> signals)
        {
            foreach (var s in signals)
            {
                _db.Signals.Add(s);
            }
        }

        public void AddOrUpdate(Common.Models.Signal signal)
        {
            MOE.Common.Models.Signal g = (from r in _db.Signals
                                          where r.SignalID == signal.SignalID 
                                          select r).FirstOrDefault();
            if (g == null)
            {
               

               _db.Signals.Add(signal);
                
            }
            else
            {
                Update(signal);
               
            }
        }



        public Signal CopySignalToNewVersion(Signal originalVersion)
        {
            Common.Models.Signal newVersion = new Signal();

            originalVersion.VersionAction = (from r in _db.VersionActions
                where r.ID == 4
                select r).FirstOrDefault();

            newVersion.VersionAction = (from r in _db.VersionActions
                where r.ID == 5
                select r).FirstOrDefault();



            newVersion.SignalID = originalVersion.SignalID;
            originalVersion.End = DateTime.Today;
            newVersion.End = LastDate;
            newVersion.Note = originalVersion.Note;
            newVersion.PrimaryName = originalVersion.PrimaryName;
            newVersion.SecondaryName = originalVersion.SecondaryName;
            newVersion.IPAddress = originalVersion.IPAddress;
            newVersion.ControllerTypeID = originalVersion.ControllerTypeID;
            newVersion.RegionID = originalVersion.RegionID;
            newVersion.Enabled = originalVersion.Enabled;
            newVersion.Latitude = originalVersion.Latitude;
            newVersion.Longitude = originalVersion.Longitude;
            newVersion.VersionID = (from r in _db.Signals
                                       select r.VersionID).Max() + 1;

            _db.Signals.Add(newVersion);
            //_db.SaveChanges();

            CopyApproaches(originalVersion, newVersion);

            return newVersion;
        }

        private void CopyApproaches(Signal signalFromDb, Signal newSignal)
        {
            List<Approach> approaches = (from r in _db.Approaches
                where r.VersionID == signalFromDb.VersionID
                select r).ToList();

            foreach (var apprFromDb in approaches)
            {
                Approach newApp = new Approach();

                newApp.SignalID = newSignal.SignalID;
                newApp.Description = apprFromDb.Description;
                newApp.DirectionTypeID = apprFromDb.DirectionTypeID;
                newApp.ProtectedPhaseNumber = apprFromDb.ProtectedPhaseNumber;
                newApp.DirectionTypeID = apprFromDb.DirectionTypeID;
                newApp.IsProtectedPhaseOverlap = apprFromDb.IsProtectedPhaseOverlap;
                newApp.MPH = apprFromDb.MPH;
                newApp.PermissivePhaseNumber = apprFromDb.PermissivePhaseNumber;
                newApp.VersionID = newSignal.VersionID;

                _db.Approaches.Add(newApp);
                //_db.SaveChanges();

                CopyDetectors(apprFromDb, newApp);
            }
        }

        private void CopyDetectors(Approach apprFromDb, Approach newApp)
        {
            var detectorsFromDb = (from r in _db.Detectors
                where r.ApproachID == apprFromDb.ApproachID
                select r).ToList();

            foreach (var detFromDb in detectorsFromDb)
            {
                var newDetector = new Detector();

                newDetector.DecisionPoint = detFromDb.DecisionPoint;
                newDetector.ApproachID = newApp.ApproachID;
                newDetector.DateAdded = DateTime.Today;
                newDetector.DetChannel = detFromDb.DetChannel;
                newDetector.DetectionHardwareID = detFromDb.DetectionHardwareID;
                newDetector.DetectorID = detFromDb.DetectorID;
                newDetector.LaneNumber = detFromDb.LaneNumber;
                newDetector.DetectorCommentIDs.AddRange(detFromDb.DetectorCommentIDs);
                newDetector.MovementTypeID = detFromDb.MovementTypeID;
                newDetector.MinSpeedFilter = detFromDb.MinSpeedFilter;
                newDetector.DistanceFromStopBar = detFromDb.DistanceFromStopBar;

                _db.Detectors.Add(newDetector);
                //_db.SaveChanges();
            }
        }

        public List<Signal> GetAllVersionsOfSignalBySignalID(string signalId)
        {
            var signals = (from r in _db.Signals
                           where r.SignalID == signalId
                           && r.VersionActionId != 3
                           select r).ToList();

            var orderedSignals = signals.OrderBy(d => d.End).ToList();

            return orderedSignals;
        }

        private VersionAction GetVersionActionByVersionActionId(int Id)
        {
            VersionAction va = (from r in _db.VersionActions
                               where r.ID == Id
                               select r).FirstOrDefault();

            return va;
        }

        public void Update(MOE.Common.Models.Signal incomingSignal)
        {
            MOE.Common.Models.Signal signalFromDatabase = (from r in _db.Signals
                                                           where r.SignalID == incomingSignal.SignalID
                                                           select r).FirstOrDefault();
            if (signalFromDatabase != null)
            {
                signalFromDatabase.VersionAction = (from r in _db.VersionActions
                                                    where r.ID == 3
                                                            select r).FirstOrDefault();



                if (incomingSignal.Approaches != null)
                {
                    foreach (MOE.Common.Models.Approach a in incomingSignal.Approaches)
                    {
                        var approach = signalFromDatabase.Approaches.FirstOrDefault(app => app.ApproachID == a.ApproachID);
                        if (approach != null)
                        {
                            if (!a.Equals(approach))
                            {
                                signalFromDatabase.Approaches.Remove(approach);
                                signalFromDatabase.Approaches.Add(a);
                                
                            }
                        }
                        else
                        {
                            signalFromDatabase.Approaches.Add(a);
                        }
                        if (a.Detectors != null)
                        {
                            foreach (MOE.Common.Models.Detector newDetector in a.Detectors)
                            {
                                var detectorFromDatabase = _db.Detectors.Where(d => d.ID == newDetector.ID).FirstOrDefault();
                                if (newDetector.DetectionTypes == null)
                                {
                                    newDetector.DetectionTypes = _db.DetectionTypes.Where(x => newDetector.DetectionTypeIDs.Contains(x.DetectionTypeID)).ToList();
                                }
                                if (detectorFromDatabase != null)
                                {
                                    if (!newDetector.Equals(detectorFromDatabase))
                                    {
                                        if (detectorFromDatabase.DetectionTypes == null)
                                        {
                                            detectorFromDatabase.DetectionTypes = new List<DetectionType>();
                                        }
                                        var deletedDetectionTypes = detectorFromDatabase.DetectionTypes
                                            .Except(newDetector.DetectionTypes).ToList<DetectionType>();
                                        var addedDetectionTypes = newDetector.DetectionTypes
                                            .Except(detectorFromDatabase.DetectionTypes).ToList<DetectionType>();

                                        deletedDetectionTypes.ForEach(delDet => detectorFromDatabase.DetectionTypes.Remove(delDet));
                                        foreach (DetectionType n in addedDetectionTypes)
                                        {

                                            detectorFromDatabase.DetectionTypes.Add(n);
                                        }

                                        a.Detectors.Remove(newDetector);
                                        a.Detectors.Add(detectorFromDatabase);
                                        
                                    }
                                }
                                else
                                {
                                    if (newDetector.DetectionTypes == null)
                                    {
                                        newDetector.DetectionTypes = _db.DetectionTypes.Where(x => newDetector.DetectionTypeIDs.Contains(x.DetectionTypeID)).ToList();
                                    }
                                    if (approach != null) approach.Detectors.Add(newDetector);
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                foreach (Common.Models.Approach a in incomingSignal.Approaches)
                {
                    foreach (Common.Models.Detector gd in a.Detectors)
                    {
                        gd.DetectionTypes = _db.DetectionTypes.Where(x => gd.DetectionTypeIDs.Contains(x.DetectionTypeID)).ToList();
                    }
                }
                _db.Signals.Add(incomingSignal);
            }
        }

        public List<Common.Models.Signal> EagerLoadAllSignals()
        {
            return _db.Signals;
        }

        public List<Common.Models.Signal> GetAllEnabledSignals()
        {
            return _db.Signals;
        }

        public List<Common.Models.Signal> GetAllSignals()
        {
            return _db.Signals;
        }

        public List<Common.Models.Signal> GetAllWithGraphDetectors()
        {
            List<Common.Models.Signal> sigsWithDetectors = new List<Common.Models.Signal>();

            foreach (var s in _db.Signals)
            {
                List<Common.Models.Detector> dets = s.GetDetectorsForSignal();

                if (dets.Count > 0)
                {
                    sigsWithDetectors.Add(s);
                }

            }

            return sigsWithDetectors;
        }

        public List<Pin> GetPinInfo()
        {
            throw new NotImplementedException();
        }

        public Common.Models.Signal GetSignalBySignalID(string signalID)
        {
            var signal = GetLatestVersionOfSignalBySignalID(signalID);

            return signal;
        }

        public Common.Models.Signal GetLatestVersionOfSignalBySignalID(string signalID)
        {
            var signal = (from r in _db.Signals
                          where r.SignalID == signalID
                          select r).OrderByDescending(x => x.End).Take(1).FirstOrDefault();



            return signal;
        }



        public SignalFTPInfo GetSignalFTPInfoByID(string signalID)
        {
            throw new NotImplementedException();
        }

        public string GetSignalLocation(string signalID)
        {
            var signal = GetSignalBySignalID(signalID);

            string location = string.Empty;
            if (signal != null)
            {
                location = signal.PrimaryName + " @ " + signal.SecondaryName;
            }

            return location;
        }

        public void Remove(Common.Models.Signal signal)
        {
            _db.Signals.Remove(signal);
        }

        public void Remove(string id)
        {
            var signal = GetSignalBySignalID(id);

            if(signal != null)
            {
                _db.Signals.Remove(signal);

            }
        }

        public List<Signal> GetLatestVerionOfAllSignals()
        {
            var signals = (from r in _db.Signals
                          where r.End ==  LastDate
                          select r).ToList();

            return signals;
        }

        public int CheckVersionWithLastDate(string signalId)
        {
            var signals = GetAllVersionsOfSignalBySignalID(signalId);

            var sigs = signals.Where(r => r.End == LastDate).ToList();

            switch (sigs.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                default:
                    return 2;
            }




        }

        public List<Signal> GetLatestVerionOfAllSignalsByControllerType(int controllerTypeId)
        {
            List<Signal> signals = (from r in _db.Signals
                where r.VersionActionId != 3 && r.End >= DateTime.Today
                && r.ControllerTypeID == controllerTypeId
                                    select r).ToList();
            
            return signals;
        }

        public Signal GetVersionOfSignalByDate(string signalId, DateTime startDate)
        {
            List<Signal> signals = (from r in _db.Signals where r.VersionActionId != 3 && r.End >= startDate
                           select r).ToList();
            Signal latestSignal = signals.OrderByDescending(r => r.End).FirstOrDefault();
            return latestSignal;
        }

        public Signal GetSignalVersionByVersionId(int versionId)
        {
            var signal = (from r in _db.Signals where r.VersionID == versionId && r.VersionActionId != 3 select r).FirstOrDefault();
            return signal;

        }

        public void SetVersionToDeleted(int versionId)
        {
            var signal = (from r in _db.Signals where r.VersionID == versionId select r).FirstOrDefault();
            if (signal != null)
            {
                signal.VersionActionId = 3;
            }
        }

        public void SetAllVersionsOfASignalToDeleted(string signalId)
        {
            var signals = from r in _db.Signals
                where r.SignalID == signalId
                select r;

            foreach (var s in signals)
            {
                s.VersionActionId = 3;
            }

        }
    }
}