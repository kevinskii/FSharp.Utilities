# FSharp.Utilities
## Overview
This set of utility functions offers convenient shortcuts for common F# data transformation tasks. It also allows for the efficient processing of data which has a known clustering. For example, consider the following simple table:

PatientID | Date | Cost
--- | --- | ---
123 | 04-FEB-11 | $600
123 | 05-FEB-11 | $100
123 | 06-FEB-11 | $250
456 | 07-JUN-13 | $350
456 | 08-AUG-13 | $200
456 | 09-AUG-14 | $400

If we want to compute the average cost for each PatientID, a common choice is to use *Seq.groupBy* or similar built-in functions. The problem is that the function cannot know where each PatientID might potentially appear, so it must process the entire list up front. This might be slow or impossible if the list is too large to fit in memory. If, however, the user knows that rows with the same PatientID will always be adjacent to one another, as commonly occurs in real-world data, this library's *groupClusteredBy* can be used in place of *groupBy* to get similar behavior with much less memory overhead.
