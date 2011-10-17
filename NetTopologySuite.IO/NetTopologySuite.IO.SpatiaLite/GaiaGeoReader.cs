﻿using System;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO
{
    internal class GaiaGeoReader
    {
        internal static IGeometry Read(byte[] blob)
        {
            //if (GaiaGeoEmptyHelper.IsEmptyBlob(blob))
            //{
            //    var g = (IGeometry)GeometryCollection.Empty.Clone();
            //    return g;
            //}
            
            //int littleEndian;
            //var  endian_arch = BitConverter.IsLittleEndian ? 1 : 0;
            //gaiaGeomCollPtr geo = NULL;
            if (blob.Length < 45)
                return null;		/* cannot be an internal BLOB WKB geometry */
            if ((GaiaGeoBlobMark)blob[0] != GaiaGeoBlobMark.GAIA_MARK_START)
                return null;		/* failed to recognize START signature */
            var size = blob.Length;
            if ((GaiaGeoBlobMark)blob[size - 1] != GaiaGeoBlobMark.GAIA_MARK_END)
                return null;		/* failed to recognize END signature */
            if ((GaiaGeoBlobMark)blob[38] != GaiaGeoBlobMark.GAIA_MARK_MBR)
                return null;		/* failed to recognize MBR signature */
            
            var gaiaImport = SetGaiaGeoParseFunctions((GaiaGeoEndianMarker) blob[1]);
            if (gaiaImport == null)
                return null;

            //geo = gaiaAllocGeomColl();
            var offset = 2;
            var srid = gaiaImport.GetInt32(blob, ref offset);
            //geo->endian_arch = (char)endian_arch;
            //geo->endian = (char)little_endian;
            //geo->blob = blob;
            //geo->size = size;
            offset = 43;
            //switch ((GaiaGeoGeometry)type)
            //{
            //    /* setting up DimensionModel */
            //    case GaiaGeoGeometry.GAIA_POINTZ:
            //    case GaiaGeoGeometry.GAIA_LINESTRINGZ:
            //    case GaiaGeoGeometry.GAIA_POLYGONZ:
            //    case GaiaGeoGeometry.GAIA_MULTIPOINTZ:
            //    case GaiaGeoGeometry.GAIA_MULTILINESTRINGZ:
            //    case GaiaGeoGeometry.GAIA_MULTIPOLYGONZ:
            //    case GaiaGeoGeometry.GAIA_GEOMETRYCOLLECTIONZ:
            //    case GaiaGeoGeometry.GAIA_COMPRESSED_LINESTRINGZ:
            //    case GaiaGeoGeometry.GAIA_COMPRESSED_POLYGONZ:
            //        geo->DimensionModel = GAIA_XY_Z;
            //        break;
            //    case GaiaGeoGeometry.GAIA_POINTM:
            //    case GaiaGeoGeometry.GAIA_LINESTRINGM:
            //    case GaiaGeoGeometry.GAIA_POLYGONM:
            //    case GaiaGeoGeometry.GAIA_MULTIPOINTM:
            //    case GaiaGeoGeometry.GAIA_MULTILINESTRINGM:
            //    case GaiaGeoGeometry.GAIA_MULTIPOLYGONM:
            //    case GaiaGeoGeometry.GAIA_GEOMETRYCOLLECTIONM:
            //    case GaiaGeoGeometry.GAIA_COMPRESSED_LINESTRINGM:
            //    case GaiaGeoGeometry.GAIA_COMPRESSED_POLYGONM:
            //        geo->DimensionModel = GAIA_XY_M;
            //        break;
            //    case GaiaGeoGeometry.GAIA_POINTZM:
            //    case GaiaGeoGeometry.GAIA_LINESTRINGZM:
            //    case GaiaGeoGeometry.GAIA_POLYGONZM:
            //    case GaiaGeoGeometry.GAIA_MULTIPOINTZM:
            //    case GaiaGeoGeometry.GAIA_MULTILINESTRINGZM:
            //    case GaiaGeoGeometry.GAIA_MULTIPOLYGONZM:
            //    case GaiaGeoGeometry.GAIA_GEOMETRYCOLLECTIONZM:
            //    case GaiaGeoGeometry.GAIA_COMPRESSED_LINESTRINGZM:
            //    case GaiaGeoGeometry.GAIA_COMPRESSED_POLYGONZM:
            //        geo->DimensionModel = GAIA_XY_Z_M;
            //        break;
            //    default:
            //        geo->DimensionModel = GAIA_XY;
            //        break;
            //};
            offset = 6;
            IEnvelope env = new Envelope(gaiaImport.GetDouble(blob, ref offset),
                                         gaiaImport.GetDouble(blob, ref offset),
                                         gaiaImport.GetDouble(blob, ref offset),
                                         gaiaImport.GetDouble(blob, ref offset));

            offset = 39;

            var type = (GaiaGeoGeometry)gaiaImport.GetInt32(blob, ref offset);
            ReadCoordinatesFunction readCoordinates = SetReadCoordinatesFunction(gaiaImport, type);


            //offset = 39;
            IGeometry geom = ParseWkbGeometry(type, blob, ref offset, readCoordinates, gaiaImport);
            //switch (type)
            //{
            //    /* parsing elementary geometries */
            //    case GAIA_POINT:
            //        ParseWkbPoint(geo, GaiaDimensionModels.GAIA_XY);
            //        break;
            //    case GAIA_POINTZ:
            //        ParseWkbPointZ(geo);
            //        break;
            //    case GAIA_POINTM:
            //        ParseWkbPointM(geo);
            //        break;
            //    case GAIA_POINTZM:
            //        ParseWkbPointZM(geo);
            //        break;
            //    case GAIA_LINESTRING:
            //        ParseWkbLine(geo);
            //        break;
            //    case GAIA_LINESTRINGZ:
            //        ParseWkbLineZ(geo);
            //        break;
            //    case GAIA_LINESTRINGM:
            //        ParseWkbLineM(geo);
            //        break;
            //    case GAIA_LINESTRINGZM:
            //        ParseWkbLineZM(geo);
            //        break;
            //    case GAIA_POLYGON:
            //        ParseWkbPolygon(geo);
            //        break;
            //    case GAIA_POLYGONZ:
            //        ParseWkbPolygonZ(geo);
            //        break;
            //    case GAIA_POLYGONM:
            //        ParseWkbPolygonM(geo);
            //        break;
            //    case GAIA_POLYGONZM:
            //        ParseWkbPolygonZM(geo);
            //        break;
            //    case GAIA_COMPRESSED_LINESTRING:
            //        ParseCompressedWkbLine(geo);
            //        break;
            //    case GAIA_COMPRESSED_LINESTRINGZ:
            //        ParseCompressedWkbLineZ(geo);
            //        break;
            //    case GAIA_COMPRESSED_LINESTRINGM:
            //        ParseCompressedWkbLineM(geo);
            //        break;
            //    case GAIA_COMPRESSED_LINESTRINGZM:
            //        ParseCompressedWkbLineZM(geo);
            //        break;
            //    case GAIA_COMPRESSED_POLYGON:
            //        ParseCompressedWkbPolygon(geo);
            //        break;
            //    case GAIA_COMPRESSED_POLYGONZ:
            //        ParseCompressedWkbPolygonZ(geo);
            //        break;
            //    case GAIA_COMPRESSED_POLYGONM:
            //        ParseCompressedWkbPolygonM(geo);
            //        break;
            //    case GAIA_COMPRESSED_POLYGONZM:
            //        ParseCompressedWkbPolygonZM(geo);
            //        break;
            //    case GAIA_MULTIPOINT:
            //    case GAIA_MULTIPOINTZ:
            //    case GAIA_MULTIPOINTM:
            //    case GAIA_MULTIPOINTZM:
            //    case GAIA_MULTILINESTRING:
            //    case GAIA_MULTILINESTRINGZ:
            //    case GAIA_MULTILINESTRINGM:
            //    case GAIA_MULTILINESTRINGZM:
            //    case GAIA_MULTIPOLYGON:
            //    case GAIA_MULTIPOLYGONZ:
            //    case GAIA_MULTIPOLYGONM:
            //    case GAIA_MULTIPOLYGONZM:
            //    case GAIA_GEOMETRYCOLLECTION:
            //    case GAIA_GEOMETRYCOLLECTIONZ:
            //    case GAIA_GEOMETRYCOLLECTIONM:
            //    case GAIA_GEOMETRYCOLLECTIONZM:
            //        ParseWkbGeometry(geo);
            //        break;
            //    default:
            //        break;
            //};
            //geo->MinX = gaiaImport64(blob + 6, little_endian, endian_arch);
            //geo->MinY = gaiaImport64(blob + 14, little_endian, endian_arch);
            //geo->MaxX = gaiaImport64(blob + 22, little_endian, endian_arch);
            //geo->MaxY = gaiaImport64(blob + 30, little_endian, endian_arch);
            //switch (type)
            //{
            //    /* setting up DeclaredType */
            //    case GAIA_POINT:
            //    case GAIA_POINTZ:
            //    case GAIA_POINTM:
            //    case GAIA_POINTZM:
            //        geo->DeclaredType = GAIA_POINT;
            //        break;
            //    case GAIA_LINESTRING:
            //    case GAIA_LINESTRINGZ:
            //    case GAIA_LINESTRINGM:
            //    case GAIA_LINESTRINGZM:
            //    case GAIA_COMPRESSED_LINESTRING:
            //    case GAIA_COMPRESSED_LINESTRINGZ:
            //    case GAIA_COMPRESSED_LINESTRINGM:
            //    case GAIA_COMPRESSED_LINESTRINGZM:
            //        geo->DeclaredType = GAIA_LINESTRING;
            //        break;
            //    case GAIA_POLYGON:
            //    case GAIA_POLYGONZ:
            //    case GAIA_POLYGONM:
            //    case GAIA_POLYGONZM:
            //    case GAIA_COMPRESSED_POLYGON:
            //    case GAIA_COMPRESSED_POLYGONZ:
            //    case GAIA_COMPRESSED_POLYGONM:
            //    case GAIA_COMPRESSED_POLYGONZM:
            //        geo->DeclaredType = GAIA_POLYGON;
            //        break;
            //    case GAIA_MULTIPOINT:
            //    case GAIA_MULTIPOINTZ:
            //    case GAIA_MULTIPOINTM:
            //    case GAIA_MULTIPOINTZM:
            //        geo->DeclaredType = GAIA_MULTIPOINT;
            //        break;
            //    case GAIA_MULTILINESTRING:
            //    case GAIA_MULTILINESTRINGZ:
            //    case GAIA_MULTILINESTRINGM:
            //    case GAIA_MULTILINESTRINGZM:
            //        geo->DeclaredType = GAIA_MULTILINESTRING;
            //        break;
            //    case GAIA_MULTIPOLYGON:
            //    case GAIA_MULTIPOLYGONZ:
            //    case GAIA_MULTIPOLYGONM:
            //    case GAIA_MULTIPOLYGONZM:
            //        geo->DeclaredType = GAIA_MULTIPOLYGON;
            //        break;
            //    case GAIA_GEOMETRYCOLLECTION:
            //    case GAIA_GEOMETRYCOLLECTIONZ:
            //    case GAIA_GEOMETRYCOLLECTIONM:
            //    case GAIA_GEOMETRYCOLLECTIONZM:
            //        geo->DeclaredType = GAIA_GEOMETRYCOLLECTION;
            //        break;
            //    default:
            //        geo->DeclaredType = GAIA_UNKNOWN;
            //        break;
            //};
            //return geo;
            if (geom != null)
            {
                geom.SRID = srid;
                //geom.Envelope = env;
            }
            return geom;
        }

        private static ReadCoordinatesFunction SetReadCoordinatesFunction(GaiaImport gaiaImport, GaiaGeoGeometry type)
        {
            gaiaImport.SetGeometryType(type);

            if (gaiaImport.Uncompressed)
            {
                if (gaiaImport.HasZ && gaiaImport.HasM)
                    return ReadXYZM;
                if (gaiaImport.HasM)
                    return ReadXYM ;
                if (gaiaImport.HasZ)
                    return ReadXYZ;
                return ReadXY;
            }

            if (gaiaImport.HasZ && gaiaImport.HasM)
                return ReadCompressedXYZM;
            if (gaiaImport.HasM)
                return ReadCompressedXYM;
            if (gaiaImport.HasZ)
                return ReadCompressedXYZ;
            return ReadXY;
        }

        private static GaiaGeoGeometry ToBaseGeometryType(GaiaGeoGeometry geometry)
        {
            return (GaiaGeoGeometry)((int)(geometry) & 0xff);
        }

        private static IGeometry ParseWkbGeometry(GaiaGeoGeometry type, byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            gaiaImport.SetGeometryType(type);

            switch (ToBaseGeometryType(type))
            {
                case GaiaGeoGeometry.GAIA_POINT:
                    return ParseWkbPoint(blob, ref offset, readCoordinates, gaiaImport);

                case GaiaGeoGeometry.GAIA_MULTIPOINT:
                    return ParseWkbMultiPoint(blob, ref offset, readCoordinates, gaiaImport);

                case GaiaGeoGeometry.GAIA_LINESTRING:
                    return ParseWkbLineString(blob, ref offset, readCoordinates, gaiaImport);

                case GaiaGeoGeometry.GAIA_MULTILINESTRING:
                    return ParseWkbMultiLineString(blob, ref offset, readCoordinates, gaiaImport);

                case GaiaGeoGeometry.GAIA_POLYGON:
                    return ParseWkbPolygon(blob, ref offset, readCoordinates, gaiaImport);

                case GaiaGeoGeometry.GAIA_MULTIPOLYGON:
                    return ParseWkbMultiPolygon(blob, ref offset, readCoordinates, gaiaImport);

                case GaiaGeoGeometry.GAIA_GEOMETRYCOLLECTION:
                    return ParseWkbGeometryCollection(blob, ref offset, readCoordinates, gaiaImport);
            }
            return null;
        }

        private static GaiaImport SetGaiaGeoParseFunctions(GaiaGeoEndianMarker gaiaGeoEndianMarker)
        {
            var conversionNeeded = false;
            switch (gaiaGeoEndianMarker)
            {
                case GaiaGeoEndianMarker.GAIA_LITTLE_ENDIAN:
                    if (!BitConverter.IsLittleEndian)
                        conversionNeeded = true;
                    break;
                case GaiaGeoEndianMarker.GAIA_BIG_ENDIAN:
                    if (BitConverter.IsLittleEndian)
                        conversionNeeded = true;
                    break;
                default:
                    /* unknown encoding; nor litte-endian neither big-endian */
                    throw new ArgumentOutOfRangeException("gaiaGeoEndianMarker");
            }

            return GaiaImport.Create(conversionNeeded);
        }

        private static IPoint ParseWkbPoint(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            return GeometryFactory.Default.CreatePoint(readCoordinates(blob, ref offset, 1, gaiaImport)[0]);
        }

        private static IMultiPoint ParseWkbMultiPoint(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            var getInt32 = gaiaImport.GetInt32;
            var getDouble = gaiaImport.GetDouble;

            var number = getInt32(blob, ref offset);
            var coords = new ICoordinate[number];
            for (var i = 0; i < number; i++)
            {
                if (blob[offset++] != (byte)GaiaGeoBlobMark.GAIA_MARK_ENTITY)
                    throw new Exception();

                var gt = getInt32(blob, ref offset);
                if ((gt & 0xff) != (int)GaiaGeoGeometry.GAIA_POINT)
                    throw new Exception();

                coords[i] = new Coordinate(getDouble(blob, ref offset),
                                           getDouble(blob, ref offset));
                if (gaiaImport.HasZ)
                    coords[i].Z = getDouble(blob, ref offset);
                if (gaiaImport.HasM)
                    coords[i].M = getDouble(blob, ref offset);
            }
            return GeometryFactory.Default.CreateMultiPoint(coords);
        }

        private delegate ILineString CreateLineStringFunction(ICoordinate[] coordinates);

        private static ILineString ParseWkbLineString(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            return ParseWkbLineString(blob, ref offset, GeometryFactory.Default.CreateLineString, readCoordinates,
                                      gaiaImport);
        }

        private static ILineString ParseWkbLineString(byte[] blob, ref int offset, CreateLineStringFunction createLineStringFunction, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            int number = gaiaImport.GetInt32(blob, ref offset);
            return createLineStringFunction(readCoordinates(blob, ref offset, number, gaiaImport));
        }

        private static IMultiLineString ParseWkbMultiLineString(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            int number = gaiaImport.GetInt32(blob, ref offset);
            var lineStrings = new ILineString[number];
            for (var i = 0; i < number; i++)
            {
                if (blob[offset++] != (byte)GaiaGeoBlobMark.GAIA_MARK_ENTITY)
                    throw new Exception();
                      
                var gt = gaiaImport.GetInt32(blob, ref offset);
                if ((gt & 0xff) != (int)GaiaGeoGeometry.GAIA_LINESTRING)
                    throw new Exception();

                lineStrings[i] = ParseWkbLineString(blob, ref offset, GeometryFactory.Default.CreateLineString, readCoordinates, gaiaImport);
            }
            return GeometryFactory.Default.CreateMultiLineString(lineStrings);
        }

        private static IPolygon ParseWkbPolygon(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            var number = gaiaImport.GetInt32(blob, ref offset) - 1;
            var shell = (ILinearRing)ParseWkbLineString(blob, ref offset, GeometryFactory.Default.CreateLinearRing, readCoordinates, gaiaImport);
            var holes = new ILinearRing[number];
            for (var i = 0; i < number; i++)
                holes[i] = (ILinearRing)ParseWkbLineString(blob, ref offset, GeometryFactory.Default.CreateLinearRing, readCoordinates, gaiaImport);

            return GeometryFactory.Default.CreatePolygon(shell, holes);
        }

        private static IGeometry ParseWkbMultiPolygon(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            var number = gaiaImport.GetInt32(blob, ref offset);
            var polygons = new IPolygon[number];
            for (var i = 0; i < number; i++)
            {
                if (blob[offset++] != (byte)GaiaGeoBlobMark.GAIA_MARK_ENTITY)
                    throw new Exception();

                var gt = gaiaImport.GetInt32(blob, ref offset);
                if ((gt & 0xff) != (int)GaiaGeoGeometry.GAIA_POLYGON)
                    throw new Exception();

                polygons[i] = ParseWkbPolygon(blob, ref offset, readCoordinates, gaiaImport);
            }
            return GeometryFactory.Default.CreateMultiPolygon(polygons);
        }

        private static IGeometryCollection ParseWkbGeometryCollection(byte[] blob, ref int offset, ReadCoordinatesFunction readCoordinates, GaiaImport gaiaImport)
        {
            var number = gaiaImport.GetInt32(blob, ref offset);
            var geometries = new IGeometry[number];
            for (var i = 0; i < number; i++)
            {
                if (blob[offset++] != (byte)GaiaGeoBlobMark.GAIA_MARK_ENTITY)
                    throw new Exception();

                geometries[i] = ParseWkbGeometry((GaiaGeoGeometry)gaiaImport.GetInt32(blob, ref offset), blob, ref offset, readCoordinates, gaiaImport);
            }
            return GeometryFactory.Default.CreateGeometryCollection(geometries);
        }

        private delegate ICoordinate[] ReadCoordinatesFunction(byte[] buffer, ref int offset, int number, GaiaImport import);

        private static ICoordinate[] ReadXY(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;
            var ret = new ICoordinate[number];
            for (var i = 0; i < number; i ++)
            {
                ret[i] = new Coordinate(getDouble(buffer, ref offset),
                                        getDouble(buffer, ref offset));
            }
            return ret;
        }

        private static ICoordinate[] ReadXYZ(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;

            var ret = new ICoordinate[number];
            for (var i = 0; i < number; i++)
            {
                ret[i] = new Coordinate(getDouble(buffer, ref offset),
                                        getDouble(buffer, ref offset),
                                        getDouble(buffer, ref offset));
            }
            return ret;
        }

        private static ICoordinate[] ReadXYM(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;
            var ret = new ICoordinate[number];
            for (var i = 0; i < number; i++)
            {
                ret[i] = new Coordinate(getDouble(buffer, ref offset),
                                        getDouble(buffer, ref offset));
                getDouble(buffer, ref offset);
            }
            return ret;
        }

        private static ICoordinate[] ReadXYZM(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;

            var ret = new ICoordinate[number];
            for (var i = 0; i < number; i++)
            {
                ret[i] = new Coordinate(getDouble(buffer, ref offset),
                                        getDouble(buffer, ref offset),
                                        getDouble(buffer, ref offset));
                getDouble(buffer, ref offset);
            }
            return ret;
        }
        private static ICoordinate[] ReadCompressedXY(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;
            var getSingle = import.GetSingle;

            var ret = new ICoordinate[number];
            ret[0] = new Coordinate(getDouble(buffer, ref offset), getDouble(buffer, ref offset));
            for (var i = 1; i < number; i++)
            {
                var fx = getSingle(buffer, ref offset);
                var fy = getSingle(buffer, ref offset);
                ret[i] = new Coordinate(ret[i].X + fx, ret[i].Y - fy);
            }
            return ret;
        }

        private static ICoordinate[] ReadCompressedXYZ(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;
            var getSingle = import.GetSingle;

            var ret = new ICoordinate[number];
            ret[0] = new Coordinate(getDouble(buffer, ref offset), getDouble(buffer, ref offset), getDouble(buffer, ref offset));
            for (var i = 1; i < number; i++)
            {
                var fx = getSingle(buffer, ref offset);
                var fy = getSingle(buffer, ref offset);
                var fz = getSingle(buffer, ref offset);
                ret[i] = new Coordinate(ret[i].X + fx, ret[i].Y - fy, ret[i].Z + fz);
            }
            return ret;
        }

        private static ICoordinate[] ReadCompressedXYM(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;
            var getSingle = import.GetSingle;

            var ret = new ICoordinate[number];
            ret[0] = new Coordinate(getDouble(buffer, ref offset), getDouble(buffer, ref offset));
            getDouble(buffer, ref offset);
            for (var i = 1; i < number; i++)
            {
                var fx = getSingle(buffer, ref offset);
                var fy = getSingle(buffer, ref offset);
                getSingle(buffer, ref offset);
                ret[i] = new Coordinate(ret[i].X + fx, ret[i].Y - fy);
            }
            return ret;
        }

        private static ICoordinate[] ReadCompressedXYZM(byte[] buffer, ref int offset, int number, GaiaImport import)
        {
            var getDouble = import.GetDouble;
            var getSingle = import.GetSingle;

            var ret = new ICoordinate[number];
            ret[0] = new Coordinate(getDouble(buffer, ref offset), getDouble(buffer, ref offset), getDouble(buffer, ref offset));
            getDouble(buffer, ref offset);
            for (var i = 1; i < number; i++)
            {
                var fx = getSingle(buffer, ref offset);
                var fy = getSingle(buffer, ref offset);
                var fz = getSingle(buffer, ref offset);
                getSingle(buffer, ref offset);
                ret[i] = new Coordinate(ret[i].X + fx, ret[i].Y - fy, ret[i].Z + fz);
            }
            return ret;
        }

    }
}