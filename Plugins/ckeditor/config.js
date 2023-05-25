/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    //config.extraPlugins = 'uploadimage';
    //config.extraPlugins = 'uploadwidget';
    //config.extraPlugins = 'filetools';
    config.allowedContent = true;
    config.protectedSource.push(/<i[^>]*><\/i>/g);
    config.protectedSource.push(/<p[^>]*><\/p>/g);
    config.protectedSource.push(/<button[^>]*><\/button>/g);
};
