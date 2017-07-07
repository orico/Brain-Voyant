using System;
using System.Collections.Generic;
using System.Text;

namespace OriBrainLearnerCore
{
    class cpp_code
    {
    }
}



//else if(ExportChoice == 3) // raw data = voxels
//    {
//        // get internal pointer to data allowing direct (fastest possible) access to data
//        // the data has been motion-corrected and spatially smoothed if this is turned on in TBV settings
//        //
//        short int **TimeCourseData = tGetTimeCourseData(); // organized as [time_point][voxel: z*dim_xy + y*dim_x +x]
		
//        // as an example that here one can use directly the full data, loop through all voxels of current time point and calculate mean 
//        int vx_i;
//        int t = tbvTime - 1;
//        double mean_of_vol = 0.0;
//        for(vx_i=0; vx_i<dim_xyz; vx_i++)
//        {
//            mean_of_vol += (double)TimeCourseData[t][vx_i];
//        }
//        mean_of_vol /= (double)dim_xyz;
//        sprintf_s(cInfo, "Mean value of current volume: %f", mean_of_vol);
//        tLogText(cInfo);

//        // save current volume to disk - we store first mini-header of dims (3 x 4-byte int) followed by volume (dim_xyz x 2-byte short int)
//        //
//        char vdat_filename[512];
//        sprintf_s(vdat_filename, "%s-%i.vdat", cProjectName, tbvTime);
//        sprintf_s(cInfo, "Saving volume data to file \"%s\" ", vdat_filename);
//        tLogText(cInfo);
		
//        ofstream vdatfile;
//        vdatfile.open(vdat_filename, ofstream::out | ofstream::binary | ofstream::trunc);
		
//        vdatfile.write((char *)&dim_x, 4*sizeof(unsigned char));
//        vdatfile.write((char *)&dim_y, 4*sizeof(unsigned char));
//        vdatfile.write((char *)&dim_z, 4*sizeof(unsigned char));
//        vdatfile.write((char *)TimeCourseData, dim_xyz*2*sizeof(unsigned char));
		
//        vdatfile.close();
//    }
//    else if(ExportChoice == 4) // export betas
//    {
//        int n_predictors_full = tGetFullNrOfPredictors();
//        int n_predictors_current = tGetCurrentNrOfPredictors();
//        sprintf_s(cInfo, "Full no. of predicotrs: %i  currently used no. of predictors: %i", n_predictors_full, n_predictors_current);
//        tLogText(cInfo);		
//        int t = tbvTime - 1;
//        sprintf_s(cInfo, "Design matrix: ");
//        char dmVal[41];
//        for(int p=0; p<n_predictors_full; p++)
//        {
//            sprintf_s(dmVal, " %6.3f", tGetValueOfDesignMatrix(p, t));
//            strcat_s(cInfo, dmVal);
//        }
//        tLogText(cInfo);
		
//        // save current state of betas to disk - we store first mini-header of nr-of-preds (betas) + dims (4 x 4-byte int) followed by nr-of-preds volumes (nr-of-preds x dim_xyz x 4-byte double)
//        //
		
//        // get internal pointer to data allowing direct (fastest possible) access to data
//        double *BetasData = tGetBetaMaps(); // organized as flat array: [n_betas*dim_xyz + z*dim_xy + y*dim_x +x]
		
//        char bdat_filename[512];
//        sprintf_s(bdat_filename, "%s-%i.bdat", cProjectName, tbvTime); 
//        sprintf_s(cInfo, "Saving %i beta maps to file \"%s\" ", n_predictors_current, bdat_filename);
//        tLogText(cInfo); 
		
//        ofstream bdatfile;
//        bdatfile.open(bdat_filename, ofstream::out | ofstream::binary | ofstream::trunc);
		
//        bdatfile.write((char *)&n_predictors_current, 4*sizeof(unsigned char));
//        bdatfile.write((char *)&dim_x, 4*sizeof(unsigned char));
//        bdatfile.write((char *)&dim_y, 4*sizeof(unsigned char));
//        bdatfile.write((char *)&dim_z, 4*sizeof(unsigned char));
//        bdatfile.write((char *)BetasData, n_predictors_current*dim_xyz*8*sizeof(unsigned char)); // "8" since betas are stored in double precision
		
//        bdatfile.close(); 
//    }
//    else // if(ExportChoice == 5) // export contrast t maps
//    {
//        int n_contrast_maps = tGetNrOfContrasts();
//        if(n_contrast_maps == 0)
//            return false;

//        // get internal pointer to data allowing direct (fastest possible) access to data
//        float *MapsData = tGetContrastMaps(); // organized as flat array: [n_maps*dim_xyz + z*dim_xy + y*dim_x +x]
		
//        // as an example, get the voxel with the highest and lowest t value for last map (this is contrast "Houses vs Faces" in example data set)
//        int map_i = n_contrast_maps - 1;
//        float tval, tval_min = 9999.0f, tval_max = -9999.0f;
//        int x, y, z, max_vox_x=0, max_vox_y=0, max_vox_z=0, min_vox_x=0, min_vox_y=0, min_vox_z=0;
		
//        for(z=0; z<dim_z; z++)
//        {	for(y=0; y<dim_y; y++)
//            {	for(x=0; x<dim_x; x++)
//                {	
//                    tval = tGetMapValueOfVoxel(map_i, x, y, z); // test of fn, slightly more efficient: tval = MapsData[map_i*dim_xyz + z*dim_xy + y*dim_x + x]
//                    if(tval > tval_max)
//                    {
//                        tval_max = tval;
//                        max_vox_x = x;
//                        max_vox_y = y;
//                        max_vox_z = z;
//                        // other (slightly more efficient) would be to use one loop "for(vx=0; vx<dim_xyz; vx++)" and to decompose from "vx" of max/min the x, y, z coordinates after loop
//                    }
//                    else if(tval < tval_min)
//                    {
//                        tval_min = tval;
//                        min_vox_x = x;
//                        min_vox_y = y;
//                        min_vox_z = z;
//                    }
//                }
//            }
//        }
//        sprintf_s(cInfo, "Minimum t value: %f at voxel x=%i, y=%i z=%i;  maximum t value: %f at voxel x=%i, y=%i z=%i", tval_min, min_vox_x, min_vox_y, min_vox_z, tval_max, max_vox_x, max_vox_y, max_vox_z);
//        tLogText(cInfo);
		
//        // save current state of maps to disk - we store first mini-header of nr-of-maps + dims (4 x 4-byte int) followed by nr-of-maps volumes (nr-of-maps x dim_xyz x 4-byte float)
//        //
//        char mdat_filename[512];
//        sprintf_s(mdat_filename, "%s-%i.mdat", cProjectName, tbvTime);
//        sprintf_s(cInfo, "Saving %i contrast t maps to file \"%s\" ", n_contrast_maps, mdat_filename);
//        tLogText(cInfo);

//        ofstream mdatfile;
//        mdatfile.open(mdat_filename, ofstream::out | ofstream::binary | ofstream::trunc);

//        mdatfile.write((char *)&n_contrast_maps, 4*sizeof(unsigned char));
//        mdatfile.write((char *)&dim_x, 4*sizeof(unsigned char));
//        mdatfile.write((char *)&dim_y, 4*sizeof(unsigned char));
//        mdatfile.write((char *)&dim_z, 4*sizeof(unsigned char));
//        mdatfile.write((char *)MapsData, n_contrast_maps*dim_xyz*4*sizeof(unsigned char));
		
//        mdatfile.close();
//    }


