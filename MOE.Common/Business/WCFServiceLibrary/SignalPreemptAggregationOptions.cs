﻿using System.Collections.Generic;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using MOE.Common.Business.Bins;
using MOE.Common.Business.DataAggregation;
using MOE.Common.Models;

namespace MOE.Common.Business.WCFServiceLibrary
{
    public class SignalPreemptAggregationOptions: AggregationMetricOptions


    {

        public SignalPreemptAggregationOptions()
        {
            MetricTypeID = 22;
        }

        protected override void GetSignalByPhaseAggregateCharts(List<Models.Signal> signals, Chart chart)
        {
            MOE.Common.Models.Repositories.IPreemptAggregationDatasRepository repo =
                MOE.Common.Models.Repositories.PreemptAggregationDatasRepositoryFactory.Create();




                //int maxPhase = (from r in aggregations select r.PreemptNumber).Max();

            for (int seriesCounter = 1; seriesCounter < 16; seriesCounter++)
            {
                Series series = new Series();
                series.Color = GetSeriesColorByNumber(seriesCounter);
                series.Name = "Preempt#" + seriesCounter.ToString();
                series.ChartArea = "ChartArea1";
                SetSeriestype(series);
                chart.Series.Add(series);
            }


            foreach (var signal in signals)
            {
                for (int seriesCounter = 1; seriesCounter < 16; seriesCounter++)
                {
                    PreemptAggregationBySignal preemptAggregationBySignal =
                        new PreemptAggregationBySignal(this, signal, BinsContainers);

                    preemptAggregationBySignal.GetPreemptTotalsBySignalByPreemptNumber(BinsContainers, seriesCounter);

                    foreach (var totals in preemptAggregationBySignal.PreemptionTotals)
                    {
                        foreach (var bin in totals.BinsContainer.Bins)
                        {
                            chart.Series[seriesCounter-1].Points.AddXY(signal.SignalID, bin.Sum);
                        }
                    }
                }
            }

                
            


        }

        protected override void GetRouteAggregateChart(List<Models.Signal> signals, Chart chart)
        {
            MOE.Common.Models.Repositories.IPreemptAggregationDatasRepository repo =
                MOE.Common.Models.Repositories.PreemptAggregationDatasRepositoryFactory.Create();


            int i = 1;
            foreach (var signal in signals)
            {
                Series series = new Series();
                series.Color = GetSeriesColorByNumber(i);
                series.Name = signal.SignalID;
                series.ChartArea = "ChartArea1";
                SetSeriestype(series);
                chart.Series.Add(series);

                PreemptAggregationBySignal preemptAggregationBySignal =
                        new PreemptAggregationBySignal(this, signal, BinsContainers);
                
                    foreach (var totals in preemptAggregationBySignal.PreemptionTotals)
                    {
                        foreach (var bin in totals.BinsContainer.Bins)
                        {
                            series.Points.AddXY(bin.Start, bin.Sum);
                        }
                    }

                i++;
            }
        }

        protected override void GetDirectionAggregateChart(Models.Signal signal, Chart chart)
        {
            
        }

        protected override void GetSignalAggregateChart(List<Models.Signal> signals, Chart chart)
        {
            throw new System.NotImplementedException();
        }

        protected override void GetSignalByDirectionAggregateChart(List<Models.Signal> signals, Chart chart)
        {
            MOE.Common.Models.Repositories.IPreemptAggregationDatasRepository repo =
                MOE.Common.Models.Repositories.PreemptAggregationDatasRepositoryFactory.Create();


            int i = 1;
            foreach (var signal in signals)
            {
                Series series = new Series();
                series.Color = GetSeriesColorByNumber(i);
                series.Name = signal.SignalID;
                series.ChartArea = "ChartArea1";
                SetSeriestype(series);
                chart.Series.Add(series);

                PreemptAggregationBySignal preemptAggregationBySignal =
                    new PreemptAggregationBySignal(this, signal, BinsContainers);

                //foreach (var totals in preemptAggregationBySignal.PreemptionTotals)
                //{
                //    foreach (var bin in totals.BinsContainer.Bins)
                //    {
                //        series.Points.AddXY(bin.Start, bin.Value);
                //    }
                //}

                i++;
            }
        }

        protected override void GetApproachAggregateChart(Models.Signal signal, Chart chart)
        {
           
        }

        protected override void GetTimeAggregateChart(Models.Signal signal, Chart chart)
        {
            


            int i = 1;
          

                Series series = new Series();
                series.Color = GetSeriesColorByNumber(i);
                series.Name = signal.SignalID;
                series.ChartArea = "ChartArea1";
                series.BorderWidth = 2;
                SetSeriestype(series);
                chart.Series.Add(series);
                i++;
                

                PreemptAggregationBySignal preemptAggregationBySignal =
                    new PreemptAggregationBySignal(this, signal, BinsContainers);

                preemptAggregationBySignal.GetPreemptsByBin(BinsContainers);

                foreach (var preemptsreempts in preemptAggregationBySignal.PreemptionTotals)
            {


                if (AggregationOperation == AggregationOperations.Sum)
                {
                    foreach (var bin in preemptsreempts.BinsContainer.Bins)
                    {
                        series.Points.AddXY(bin.Start, bin.Sum);
                    }
                }
        

            }
            

    }
}}