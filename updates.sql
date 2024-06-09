use [Ramallah]

/*  Before you execute 
-- Remove autoincrement from the table of categories
*/
INSERT INTO Ramallah.dbo.categories(
    Id,Name, ArName, Slug, Thumb, Priority, ParentId, TemplateId,
    ItemsPerPage, Active, Publish, TypeId, Description, ArDescription,
    LangId, ShowAsMain, ShowInSiteMap, ShowDescription, ShowTitle,
    ShowThumb, ShowInPath, ShowInSearch, ShowDate, ShowInCatList, UserId, Deleted
)
SELECT
	c_id as Id,
    c_name AS Name,
    c_ar_name AS ArName,
    NULL AS Slug, 
    c_thumb AS Thumb,
    c_priority AS Priority,
    c_parent AS ParentId,
    NULL AS TemplateId,
    c_itemsperpage AS ItemsPerPage,
    1 AS Active, 
    1 AS Publish, 
    c_type AS TypeId,
    c_description AS Description,
    c_ar_description AS ArDescription,
    1 AS LangId,
    c_showasmenu AS ShowAsMain,
    c_showinsitemap AS ShowInSiteMap,
    1 AS ShowDescription, 
    1 AS ShowTitle,
    1 AS ShowThumb, 
    c_showinpath AS ShowInPath,
    0 AS ShowInSearch, 
    0 AS ShowDate, 
    1 AS ShowInCatList, 
    1 AS UserId, 
    c_deleted AS Deleted
FROM ramallahold.dbo.categories;

update Ramallah.dbo.categories set TemplateId = 1009;
/* 
-- Return the autoIncrement to Table (Categories)

before you move 
-- Truncate TagsRel and PagesCategories
-- Remove Pages autoincrement
*/

INSERT INTO Ramallah.dbo.Pages (
    PageId, TranslateId, Title, PageDate, AddDate, LangId, Body, Slug, Url, RedirectUrl,
    Thumb, Thumb2, ShowThumb, MetaDescription, MetaKeywords, TemplateId, Priority,
    Publish, Active, AsMenu, ShowAsMain, Parent, ShowInSearchPage, ShowInSiteMap,
    ShowDate, AllowComment, ShowAsRelated, Summary, ValidDate, SubTitle, Gallery,
    ShowRelated, Sticky, Deleted, Archive, Views, Video, Audio, UserId, EditedBy, LastEdit, FormId
)
SELECT
    p_id AS PageId,
    NULL AS TranslateId, -- Assuming TranslateId is a new column and setting it to NULL or a default value
    p_title AS Title,
    p_date AS PageDate,
    p_date AS AddDate, -- Assuming AddDate is a new column and setting it to the current date
    2 AS LangId,
    p_body AS Body,
    NULL AS Slug, -- Assuming Slug is a new column and setting it to NULL or a default value
    p_url AS Url,
    NULL AS RedirectUrl, -- Assuming RedirectUrl is a new column and setting it to NULL or a default value
    p_thumb AS Thumb,
    p_thumb2 AS Thumb2,
    p_showthumb AS ShowThumb,
    p_metadescription AS MetaDescription,
    p_metakeywords AS MetaKeywords,
    p_template AS TemplateId,
    p_priority AS Priority,
    p_active AS Publish, -- Assuming Publish is a new column and setting it to NULL or a default value
    p_active AS Active,
    0 AS AsMenu,
    p_showasmain AS ShowAsMain,
    p_parent AS Parent,
    p_showinsearchpage AS ShowInSearchPage,
    p_showinsitemap AS ShowInSiteMap,
    p_showdate AS ShowDate,
    p_comment AS AllowComment,
    0 AS ShowAsRelated,
    p_summary AS Summary,
    p_valid_date AS ValidDate,
    p_subtitle AS SubTitle,
    p_gallery AS Gallery,
    p_showrelated AS ShowRelated,
    0 AS Sticky,
    p_deleted AS Deleted,
    0 AS Archive, -- Assuming Archive is a new column and setting it to NULL or a default value
    p_views AS Views,
    p_video AS Video,
    NULL AS Audio, -- Assuming Audio is a new column and setting it to NULL or a default value
    p_userid AS UserId,
    p_edited_by AS EditedBy,
    p_last_edit AS LastEdit,
    NULL AS FormId -- Assuming FormId is a new column and setting it to NULL or a default value
FROM ramallahOld.dbo.pages;


INSERT INTO Ramallah.dbo.PagesCategories (
   LangId,PageId,CategoryId
)
SELECT
    2 As LangId, p_id, c_id
FROM ramallahOld.dbo.page_category;

update Ramallah.dbo.Pages set TemplateId = 1006;

/*
-- Return the Auto to table Pages
-- Remove the Identity from table Menus
*/

truncate table Ramallah.dbo.Menus;

INSERT INTO Ramallah.dbo.Menus (
    Id, Name, LocationId, Target, ParentId, Priority, Link, LangId, Active, UserId, Deleted
)
SELECT
    m_id AS Id,
    m_name AS Name,
    m_location AS LocationId,
    '_self' AS Target, -- Assuming Target is a new column and setting it to NULL or a default value
    m_parent AS ParentId,
    m_priority AS Priority,
    m_link AS Link,
    2 AS LangId, -- Assuming LangId is a new column and setting it to NULL or a default value
    1 AS Active, -- Assuming Active is a new column and setting it to NULL or a default value
    1 AS UserId, -- Assuming UserId is a new column and setting it to NULL or a default value
    m_deleted AS Deleted
FROM ramallahOld.dbo.ar_menus;

update Ramallah.dbo.Menus set ParentId=null where ParentId = 0;

update Ramallah.dbo.Menus set LocationId =5 where Id = 1340;

update Ramallah.dbo.Menus set LocationId =3 where LocationId=5;
update Ramallah.dbo.Menus set LocationId =4 where LocationId=6;
/*
-- return the Identity to table Menus

-- Remove the Identity from column ID of table Files
*/
truncate table Ramallah.dbo.Files;

INSERT INTO Ramallah.dbo.Files (
    Id, CatId, Name, ArName, Year, Parent, Publish, Active, Thumb, LangId, Description,
    ArDescription, FilePath, Source, Priority, ShowHome, AllowComment, UserId, Date, AddDate,
    UpdatedAt, Deleted
)
SELECT
    f_id AS Id,
    f_catid AS CatId,
    f_title AS Name,
    f_ar_title AS ArName,
    NULL AS Year, -- Assuming Year is a new column and setting it to NULL or a default value
    0 AS Parent, -- Assuming Parent is a new column and setting it to NULL or a default value
    1 AS Publish, -- Assuming Publish is a new column and setting it to NULL or a default value
    f_active AS Active,
    f_thumb AS Thumb,
    f_lang AS LangId,
    f_description AS Description,
    f_ar_description AS ArDescription,
    f_file AS FilePath,
    f_source AS Source,
    f_priority AS Priority,
    0 AS ShowHome, -- Assuming ShowHome is a new column and setting it to NULL or a default value
    0 AS AllowComment, -- Assuming AllowComment is a new column and setting it to NULL or a default value
    1 AS UserId, -- Assuming UserId is a new column and setting it to NULL or a default value
    GETDATE() AS Date,
    GETDATE() AS AddDate,
    NULL AS UpdatedAt, -- Assuming UpdatedAt is a new column and setting it to NULL or a default value
    f_deleted AS Deleted
FROM ramallahOld.dbo.files;

/*
-- return the Identity to table Files

*/

update Pages set Thumb = Replace(Thumb,'userfiles/','files/'),Thumb2 = Replace(Thumb2,'userfiles/','files/');
update Pages set Body = Replace(Body,'/userfiles/','/files/');

update Categories set Thumb = Replace(Thumb,'userfiles/','files/');

update Files set Thumb = Replace(Thumb,'userfiles/','files/'),FilePath = Replace(FilePath,'userfiles/','files/');
update Pages set TemplateId =1010 where PageId in (select PageId from PagesCategories where CategoryId=1097);