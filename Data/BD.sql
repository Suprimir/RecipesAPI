-- ============================================
-- COOKFLOW DATABASE SCHEMA - VERSIÓN ACTUALIZADA
-- PostgreSQL 15+
-- ============================================

-- Extensiones necesarias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- Para búsqueda full-text optimizada

-- ============================================
-- TABLA: users
-- ============================================
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    bio TEXT,
    profile_image_url VARCHAR(500),
    banner_image_url VARCHAR(500),
    is_email_verified BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT email_format CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT username_format CHECK (username ~* '^[a-zA-Z0-9_]{3,50}$')
);

-- Índices para users
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_created_at ON users(created_at DESC);

-- ============================================
-- TABLA: categories
-- ============================================
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    icon_name VARCHAR(50),
    display_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- TABLA: tags
-- ============================================
CREATE TABLE tags (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    slug VARCHAR(50) UNIQUE NOT NULL,
    type VARCHAR(30) NOT NULL,
    color VARCHAR(7),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT tag_type_check CHECK (type IN ('dietary', 'cuisine', 'difficulty', 'meal_type', 'cooking_method'))
);

CREATE INDEX idx_tags_type ON tags(type);

-- ============================================
-- TABLA: recipes
-- ============================================
CREATE TABLE recipes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id INTEGER REFERENCES categories(id) ON DELETE SET NULL,
    
    title VARCHAR(200) NOT NULL,
    description TEXT,
    cover_image_url VARCHAR(500),
    
    prep_time_minutes INTEGER,
    cook_time_minutes INTEGER,
    total_time_minutes INTEGER GENERATED ALWAYS AS (prep_time_minutes + cook_time_minutes) STORED,
    
    difficulty_level VARCHAR(20) DEFAULT 'easy',
    is_public BOOLEAN DEFAULT FALSE,
    favorites_count INTEGER DEFAULT 0,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    published_at TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT difficulty_check CHECK (difficulty_level IN ('easy', 'medium', 'hard')),
    CONSTRAINT prep_time_positive CHECK (prep_time_minutes >= 0),
    CONSTRAINT cook_time_positive CHECK (cook_time_minutes >= 0)
);

-- Índices para recipes
CREATE INDEX idx_recipes_user_id ON recipes(user_id);
CREATE INDEX idx_recipes_category_id ON recipes(category_id);
CREATE INDEX idx_recipes_is_public ON recipes(is_public) WHERE is_public = TRUE;
CREATE INDEX idx_recipes_created_at ON recipes(created_at DESC);
CREATE INDEX idx_recipes_favorites ON recipes(favorites_count DESC);
CREATE INDEX idx_recipes_title_search ON recipes USING gin(to_tsvector('spanish', title));
CREATE INDEX idx_recipes_difficulty ON recipes(difficulty_level);

-- ============================================
-- TABLA: recipe_steps (CON STEP_TYPE AGREGADO)
-- ============================================
CREATE TABLE recipe_steps (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    
    step_number INTEGER NOT NULL,
    step_type VARCHAR(30) NOT NULL DEFAULT 'preparation', -- NUEVO CAMPO
    title VARCHAR(200),
    description TEXT NOT NULL,
    image_url VARCHAR(500),
    duration_minutes INTEGER,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT step_number_positive CHECK (step_number > 0),
    CONSTRAINT duration_positive CHECK (duration_minutes IS NULL OR duration_minutes >= 0),
    CONSTRAINT step_type_check CHECK (step_type IN (
        'preparation',      -- Preparación general
        'cutting',          -- Cortar/Picar
        'mixing',           -- Mezclar
        'heating',          -- Calentar (estufa)
        'baking',           -- Hornear
        'frying',           -- Freír
        'boiling',          -- Hervir
        'grilling',         -- Asar/Parrilla
        'blending',         -- Licuar
        'marinating',       -- Marinar/Reposar
        'refrigerating',    -- Refrigerar
        'resting',          -- Dejar reposar (temperatura ambiente)
        'serving',          -- Servir/Emplatar
        'decorating',       -- Decorar
        'other'             -- Otro
    )),
    UNIQUE(recipe_id, step_number)
);

-- Índices para recipe_steps
CREATE INDEX idx_recipe_steps_recipe_id ON recipe_steps(recipe_id, step_number);
CREATE INDEX idx_recipe_steps_type ON recipe_steps(step_type);

-- ============================================
-- TABLA: ingredients
-- ============================================
CREATE TABLE ingredients (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    category VARCHAR(50),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_ingredients_name_search ON ingredients USING gin(to_tsvector('spanish', name));
CREATE INDEX idx_ingredients_category ON ingredients(category);

-- ============================================
-- TABLA: recipe_ingredients
-- ============================================
CREATE TABLE recipe_ingredients (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    ingredient_id INTEGER NOT NULL REFERENCES ingredients(id) ON DELETE CASCADE,
    
    quantity DECIMAL(10, 2),
    unit VARCHAR(50),
    notes TEXT,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT quantity_positive CHECK (quantity IS NULL OR quantity > 0),
    UNIQUE(recipe_id, ingredient_id)
);

CREATE INDEX idx_recipe_ingredients_recipe_id ON recipe_ingredients(recipe_id);
CREATE INDEX idx_recipe_ingredients_ingredient_id ON recipe_ingredients(ingredient_id);

-- ============================================
-- TABLA: recipe_tags
-- ============================================
CREATE TABLE recipe_tags (
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    tag_id INTEGER NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (recipe_id, tag_id)
);

CREATE INDEX idx_recipe_tags_recipe_id ON recipe_tags(recipe_id);
CREATE INDEX idx_recipe_tags_tag_id ON recipe_tags(tag_id);

-- ============================================
-- TABLA: favorites
-- ============================================
CREATE TABLE favorites (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(user_id, recipe_id)
);

CREATE INDEX idx_favorites_user_id ON favorites(user_id, created_at DESC);
CREATE INDEX idx_favorites_recipe_id ON favorites(recipe_id);

-- ============================================
-- TABLA: ratings
-- ============================================
CREATE TABLE ratings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    
    rating INTEGER NOT NULL,
    review TEXT,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT rating_range CHECK (rating >= 1 AND rating <= 5),
    UNIQUE(user_id, recipe_id)
);

CREATE INDEX idx_ratings_recipe_id ON ratings(recipe_id);
CREATE INDEX idx_ratings_user_id ON ratings(user_id);
CREATE INDEX idx_ratings_rating ON ratings(rating);

-- ============================================
-- TABLA: recipe_likes (likes independientes de favorites)
-- ============================================
CREATE TABLE recipe_likes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(user_id, recipe_id)
);

CREATE INDEX idx_recipe_likes_recipe_id ON recipe_likes(recipe_id);
CREATE INDEX idx_recipe_likes_user_id ON recipe_likes(user_id);

-- ============================================
-- TABLA: recipe_comments (comentarios con threading opcional)
-- ============================================
CREATE TABLE recipe_comments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    recipe_id UUID NOT NULL REFERENCES recipes(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    content VARCHAR(1000) NOT NULL,
    parent_comment_id UUID REFERENCES recipe_comments(id) ON DELETE CASCADE,
    is_edited BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_recipe_comments_recipe_id ON recipe_comments(recipe_id);
CREATE INDEX idx_recipe_comments_user_id ON recipe_comments(user_id);
CREATE INDEX idx_recipe_comments_parent_comment_id ON recipe_comments(parent_comment_id);

-- ============================================
-- TABLA: follows
-- ============================================
-- (Se mantiene/crea aquí para el esquema global)
CREATE TABLE follows (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    follower_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    following_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT no_self_follow CHECK (follower_id != following_id),
    UNIQUE(follower_id, following_id)
);

CREATE INDEX idx_follows_follower_id ON follows(follower_id);
CREATE INDEX idx_follows_following_id ON follows(following_id);

-- ============================================
-- TABLA: notifications
-- ============================================
CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    
    type VARCHAR(50) NOT NULL,
    title VARCHAR(200) NOT NULL,
    message TEXT,
    
    related_user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    related_recipe_id UUID REFERENCES recipes(id) ON DELETE CASCADE,
    
    is_read BOOLEAN DEFAULT FALSE,
    read_at TIMESTAMP WITH TIME ZONE,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT notification_type_check CHECK (type IN (
        'new_follower', 'recipe_rated', 'recipe_commented', 
        'recipe_cooked', 'recipe_featured', 'new_recipe_from_following'
    ))
);

CREATE INDEX idx_notifications_user_id ON notifications(user_id, created_at DESC);
CREATE INDEX idx_notifications_is_read ON notifications(user_id, is_read) WHERE is_read = FALSE;

-- ============================================
-- TABLA: refresh_tokens
-- ============================================
-- Usamos columna booleana y triggers para evitar problemas con expresiones no inmutables
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) UNIQUE NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP WITH TIME ZONE,
    replaced_by_token VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    
    CONSTRAINT expires_after_creation CHECK (expires_at > created_at)
);

CREATE INDEX idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token);
CREATE INDEX idx_refresh_tokens_active ON refresh_tokens(user_id, is_active) WHERE is_active = TRUE;
CREATE INDEX idx_refresh_tokens_expires ON refresh_tokens(expires_at);

-- ============================================
-- TABLA: password_reset_tokens
-- ============================================
CREATE TABLE password_reset_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) UNIQUE NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    used_at TIMESTAMP WITH TIME ZONE,
    is_valid BOOLEAN DEFAULT TRUE,
    
    CONSTRAINT expires_after_creation_reset CHECK (expires_at > created_at)
);

CREATE INDEX idx_password_reset_tokens_user_id ON password_reset_tokens(user_id);
CREATE INDEX idx_password_reset_tokens_token ON password_reset_tokens(token);
CREATE INDEX idx_password_reset_tokens_valid ON password_reset_tokens(user_id, is_valid) WHERE is_valid = TRUE;
CREATE INDEX idx_password_reset_tokens_expires ON password_reset_tokens(expires_at);

-- ============================================
-- TABLA: email_verification_tokens
-- ============================================
CREATE TABLE email_verification_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) UNIQUE NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    verified_at TIMESTAMP WITH TIME ZONE,
    is_valid BOOLEAN DEFAULT TRUE,
    
    CONSTRAINT expires_after_creation_email CHECK (expires_at > created_at)
);

CREATE INDEX idx_email_verification_tokens_user_id ON email_verification_tokens(user_id);
CREATE INDEX idx_email_verification_tokens_token ON email_verification_tokens(token);
CREATE INDEX idx_email_verification_tokens_valid ON email_verification_tokens(user_id, is_valid) WHERE is_valid = TRUE;
CREATE INDEX idx_email_verification_tokens_expires ON email_verification_tokens(expires_at);

-- ============================================
-- FUNCIONES Y TRIGGERS
-- ============================================

-- Función para actualizar updated_at automáticamente
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Triggers para updated_at
CREATE TRIGGER update_users_updated_at 
    BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_recipes_updated_at 
    BEFORE UPDATE ON recipes
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_recipe_steps_updated_at 
    BEFORE UPDATE ON recipe_steps
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_ratings_updated_at 
    BEFORE UPDATE ON ratings
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_recipe_comments_updated_at
    BEFORE UPDATE ON recipe_comments
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- ============================================
-- FUNCIONES/ TRIGGERS PARA AUTH TOKENS (refresh/password_reset/email_verification)
-- ============================================

-- Función para actualizar is_active en refresh_tokens
CREATE OR REPLACE FUNCTION update_refresh_token_active()
RETURNS TRIGGER AS $$
BEGIN
    NEW.is_active := (NEW.revoked_at IS NULL AND NEW.expires_at > CURRENT_TIMESTAMP);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_refresh_token_active
BEFORE INSERT OR UPDATE ON refresh_tokens
FOR EACH ROW
EXECUTE FUNCTION update_refresh_token_active();

-- Función para actualizar is_valid en password_reset_tokens
CREATE OR REPLACE FUNCTION update_password_reset_valid()
RETURNS TRIGGER AS $$
BEGIN
    NEW.is_valid := (NEW.used_at IS NULL AND NEW.expires_at > CURRENT_TIMESTAMP);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_password_reset_valid
BEFORE INSERT OR UPDATE ON password_reset_tokens
FOR EACH ROW
EXECUTE FUNCTION update_password_reset_valid();

-- Función para actualizar is_valid en email_verification_tokens
CREATE OR REPLACE FUNCTION update_email_verification_valid()
RETURNS TRIGGER AS $$
BEGIN
    NEW.is_valid := (NEW.verified_at IS NULL AND NEW.expires_at > CURRENT_TIMESTAMP);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_email_verification_valid
BEFORE INSERT OR UPDATE ON email_verification_tokens
FOR EACH ROW
EXECUTE FUNCTION update_email_verification_valid();

-- ============================================
-- Función para incrementar contador de favoritos
-- ============================================
CREATE OR REPLACE FUNCTION increment_favorites_count()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE recipes 
    SET favorites_count = favorites_count + 1 
    WHERE id = NEW.recipe_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_increment_favorites 
    AFTER INSERT ON favorites
    FOR EACH ROW EXECUTE FUNCTION increment_favorites_count();

-- Función para decrementar contador de favoritos
CREATE OR REPLACE FUNCTION decrement_favorites_count()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE recipes 
    SET favorites_count = favorites_count - 1 
    WHERE id = OLD.recipe_id AND favorites_count > 0;
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_decrement_favorites 
    AFTER DELETE ON favorites
    FOR EACH ROW EXECUTE FUNCTION decrement_favorites_count();

-- ============================================
-- Función para actualizar published_at
-- ============================================
CREATE OR REPLACE FUNCTION set_published_at()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.is_public = TRUE AND OLD.is_public = FALSE THEN
        NEW.published_at = CURRENT_TIMESTAMP;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_set_published_at 
    BEFORE UPDATE ON recipes
    FOR EACH ROW EXECUTE FUNCTION set_published_at();

-- ============================================
-- DATOS DE EJEMPLO (SEEDS)
-- ============================================

-- Categorías predefinidas
INSERT INTO categories (name, description, icon_name, display_order) VALUES
('Desayuno', 'Recetas para comenzar el día', 'sunrise', 1),
('Almuerzo', 'Comidas del mediodía', 'sun', 2),
('Cena', 'Recetas para la noche', 'moon', 3),
('Postres', 'Dulces y postres', 'cake', 4),
('Snacks', 'Bocadillos y aperitivos', 'cookie', 5),
('Bebidas', 'Bebidas y cócteles', 'glass', 6),
('Ensaladas', 'Ensaladas frescas', 'salad', 7),
('Sopas', 'Sopas y caldos', 'bowl', 8);

-- Tags predefinidos
INSERT INTO tags (name, slug, type, color) VALUES
-- Dietary
('Vegetariano', 'vegetariano', 'dietary', '#4CAF50'),
('Vegano', 'vegano', 'dietary', '#8BC34A'),
('Sin Gluten', 'sin-gluten', 'dietary', '#FF9800'),
('Sin Lactosa', 'sin-lactosa', 'dietary', '#2196F3'),
('Keto', 'keto', 'dietary', '#9C27B0'),
('Bajo en Calorías', 'bajo-calorias', 'dietary', '#00BCD4'),

-- Cuisine
('Mexicana', 'mexicana', 'cuisine', '#C62828'),
('Italiana', 'italiana', 'cuisine', '#388E3C'),
('Asiática', 'asiatica', 'cuisine', '#F57C00'),
('Mediterránea', 'mediterranea', 'cuisine', '#0288D1'),
('Americana', 'americana', 'cuisine', '#5D4037'),

-- Difficulty
('Fácil', 'facil', 'difficulty', '#4CAF50'),
('Intermedio', 'intermedio', 'difficulty', '#FF9800'),
('Difícil', 'dificil', 'difficulty', '#F44336'),

-- Cooking Method
('Al Horno', 'al-horno', 'cooking_method', '#FF5722'),
('Frito', 'frito', 'cooking_method', '#FFC107'),
('A la Parrilla', 'a-la-parrilla', 'cooking_method', '#795548'),
('Crudo', 'crudo', 'cooking_method', '#4CAF50');

-- Ingredientes comunes
INSERT INTO ingredients (name, category) VALUES
-- Vegetales
('Cebolla', 'vegetable'),
('Ajo', 'vegetable'),
('Tomate', 'vegetable'),
('Pimiento', 'vegetable'),
('Zanahoria', 'vegetable'),
('Papa', 'vegetable'),
('Lechuga', 'vegetable'),

-- Proteínas
('Pollo', 'meat'),
('Res', 'meat'),
('Cerdo', 'meat'),
('Pescado', 'meat'),
('Huevo', 'protein'),

-- Lácteos
('Leche', 'dairy'),
('Queso', 'dairy'),
('Mantequilla', 'dairy'),
('Crema', 'dairy'),

-- Granos
('Arroz', 'grain'),
('Pasta', 'grain'),
('Harina', 'grain'),
('Pan', 'grain'),

-- Especias
('Sal', 'spice'),
('Pimienta', 'spice'),
('Comino', 'spice'),
('Orégano', 'spice'),
('Cilantro', 'spice'),

-- Otros
('Aceite', 'oil'),
('Agua', 'liquid'),
('Azúcar', 'sweetener');

-- ============================================
-- COMENTARIOS PARA DOCUMENTACIÓN
-- ============================================

COMMENT ON TABLE users IS 'Usuarios registrados en la aplicación';
COMMENT ON TABLE recipes IS 'Recetas creadas por usuarios';
COMMENT ON TABLE recipe_steps IS 'Pasos individuales de cada receta con tipo de acción';
COMMENT ON COLUMN recipe_steps.step_type IS 'Tipo de paso: preparation, cutting, mixing, heating, baking, frying, boiling, grilling, blending, marinating, refrigerating, resting, serving, decorating, other';
COMMENT ON TABLE ingredients IS 'Catálogo de ingredientes disponibles';
COMMENT ON TABLE recipe_ingredients IS 'Ingredientes específicos de cada receta con cantidades';
COMMENT ON TABLE favorites IS 'Recetas marcadas como favoritas por usuarios';
COMMENT ON TABLE ratings IS 'Calificaciones de 1-5 estrellas para recetas';
COMMENT ON TABLE follows IS 'Relación de seguimiento entre usuarios';
COMMENT ON TABLE notifications IS 'Notificaciones del sistema para usuarios';