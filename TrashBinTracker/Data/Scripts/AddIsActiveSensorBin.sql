IF COL_LENGTH('TrashBins', 'IsActiveSensorBin') IS NULL
BEGIN
    ALTER TABLE TrashBins
    ADD IsActiveSensorBin bit NOT NULL
        CONSTRAINT DF_TrashBins_IsActiveSensorBin DEFAULT 0;
END;
