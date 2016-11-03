USE $MAIN$;

-- Add config
INSERT IGNORE INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailConfirmedNotification', 'string', 'Email confirmed notification to support', 'Email confirmed notification to support', 'Dear support,\n\nThis is an automatic email to inform you that user $(USERNAME) has just confirmed his email address ($(EMAIL)) on the TEP platform.\n', '0');
INSERT IGNORE INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('DataGatewayBaseUrl', 'string', 'Data Gateway Base Url', 'Data Gateway Base Url', 'https://store.terradue.com', '0');
INSERT IGNORE INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('DomainThematicPrefix', 'string', 'Domain identifier prefix for Thematic groups', 'Domain identifier prefix for Thematic groups', 'hydro-', '0');
UPDATE config SET `value`='Dear user,\n\nYour account $(USERNAME) has been created on $(SITENAME).\nWe must verify the authenticity of your email address.\nTo do so, please click on the following link: $(ACTIVATIONURL)\n\nWith our best regards, the Operations Support team at Terradue' WHERE `name`='RegistrationMailBody';
UPDATE config SET `value`='[$(SITENAME)] - Registration' WHERE `name`='RegistrationMailSubject';

-- RESULT

-- Add type
SET @type_id = (SELECT id FROM type WHERE class='Terradue.Tep.DataPackage, Terradue.Tep');
INSERT INTO type (`id_super`, `pos`, `class`, `caption_sg`, `caption_pl`, `keyword`) VALUES (@type_id, '0', 'Terradue.Tep.ThematicApplication, Terradue.Tep', 'Thematic Apps', 'Thematic Apps', 'apps');
-- RESULT

-- Update priv for Datapackage ... \
SET @type_id = (SELECT id FROM type WHERE class='Terradue.Tep.DataPackage, Terradue.Tep');
SET @priv_pos = (SELECT MAX(pos) FROM priv);
INSERT INTO priv (id_type, identifier, operation, pos, name, enable_log) VALUES
    (@type_id, 'datapackage-s', 's', @priv_pos + 1, 'DataPackage: search', 1);
UPDATE priv SET identifier='datapackage-v' WHERE id_type=@type_id AND operation='v';
UPDATE priv SET identifier='datapackage-c' WHERE id_type=@type_id AND operation='c';
UPDATE priv SET identifier='datapackage-m' WHERE id_type=@type_id AND operation='m';
UPDATE priv SET identifier='datapackage-M' WHERE id_type=@type_id AND operation='M';
UPDATE priv SET identifier='datapackage-d' WHERE id_type=@type_id AND operation='d';
-- RESULT

-- Update priv for Wpsjob ... \
SET @type_id = (SELECT id FROM type WHERE class='Terradue.Tep.WpsJob, Terradue.Tep');
SET @priv_pos = (SELECT MAX(pos) FROM priv);
INSERT INTO priv (id_type, identifier, operation, pos, name, enable_log) VALUES
    (@type_id, 'wpsjob-s', 's', @priv_pos + 1, 'WpsJob: search', 1);
UPDATE priv SET identifier='wpsjob-v' WHERE id_type=@type_id AND operation='v';
UPDATE priv SET identifier='wpsjob-c' WHERE id_type=@type_id AND operation='c';
UPDATE priv SET identifier='wpsjob-m' WHERE id_type=@type_id AND operation='m';
UPDATE priv SET identifier='wpsjob-M' WHERE id_type=@type_id AND operation='M';
UPDATE priv SET identifier='wpsjob-d' WHERE id_type=@type_id AND operation='d';
-- RESULT

-- Add domain for wpsjob table
ALTER TABLE wpsjob ADD COLUMN id_domain INT UNSIGNED NULL;
ALTER TABLE wpsjob ADD CONSTRAINT fk_wpsjob_domain FOREIGN KEY (id_domain) REFERENCES domain(id) ON DELETE SET NULL;
-- RESULT