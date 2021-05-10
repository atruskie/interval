namespace Math.Interval
{
    public enum Topology : byte
    {
        /*
         * Our flags are defined like this to ensure the default value is [a,b).
         * The meaning of bit 1 is: Is the left exclusive? (1 yes, 0 no)
         * The meaning of bit 2 is: Is the right inclusive? (1 yes, 0 no)
         *
         * Note bit 1 is on the right.
         */

        /// <summary> Endpoints not included ( min < x < max ) </summay>
        Open = 0b0_1,

        /// <summary> Lower endpoint is included, upper is not ( min ≤ x < max ) </summay>
        LeftClosedRightOpen = 0b0_0,

        /// <summary> Lower endpoint is not included, upper is ( min < x ≤ max ) </summay>
        LeftOpenRightClosed = 0b1_1,

        /// <summary> Endpoints are included ( min ≤ x ≤ max ) </summay>
        Closed = 0b1_0,


        /// <summary> Endpoints not included ( min < x < max ) </summay>
        Exclusive = Open,

        /// <summary> Lower endpoint is included, upper is not ( min ≤ x < max ) </summay>
        MinimumInclusiveMaximumExclusive = LeftClosedRightOpen,

        /// <summary> Lower endpoint is not included, upper is ( min < x ≤ max ) </summay>
        MinimumExclusiveMaximumInclusive = LeftOpenRightClosed,

        /// <summary> Endpoints are included ( min ≤ x ≤ max ) </summay>
        Inclusive = Closed,

        /// <summary> Lower endpoint is included, upper is not ( min ≤ x < max ) </summay>
        Default = LeftClosedRightOpen,
    }
}